/**
 * 
 * 关于大厅桌子请求的处理逻辑
 */
var pomelo = require('pomelo'),
    CODE = require('../../consts/code.js'),
    CONSTS = require('../../consts/consts.js'),
    ROUTE = require('../../consts/route.js'),
    Table = require('../entity/table.js'),
    channelService = require('../../service/channelService.js');

var tableManager = module.exports;

/*
* 选择游戏模式
* */
tableManager.selectMode = function(cb) {
  // 获取相应模式的桌子列表数据并返回
  var tables = pomelo.app.globalData.tables;
  if(!tables) {
    return cb(new Error(CONSTS.ERR.LOGIC_ERR), {code: CODE.DATA_ERROR});
  }

  var resData = [];
  for(var i in tables) {resData.push(tables[i]);}

  cb(null, {code: CODE.SUCCESS, tables: resData});
};

/*
* 选择某一个桌子的位置坐下
* */
tableManager.sitDown = function(uid, tableId, chairId, cb) {
  // 判断请求参数有效性
  if(!tableId || (chairId != CONSTS.CHAIR.LEFT && chairId != CONSTS.CHAIR.RIGHT)) {
    return cb(new Error(CONSTS.ERR.LOGIC_ERR), {code: CODE.DATA_ERROR});
  }

  // 这个座位已经有人坐了
  var tableData = pomelo.app.globalData.tables[tableId];
  if(tableData[chairId]) {return cb(null, {code: CODE.SITTED});}

  // 在这个位置坐下，将玩家加入此频道
  tableData[chairId] = {uid: uid, enterTime: 0};
  channelService.add(uid, pomelo.app.globalData.players[uid].sid, tableId, chairId);
  pomelo.app.globalData.players[uid].tableId = tableId;
  pomelo.app.globalData.players[uid].chairId = chairId;

  // 自动取分
  transScoreAuto(uid);
  var backMsg = {
    code: CODE.SUCCESS,
    tableInfo : {tableId: tableId,tableName: '有来有去'},
    updatePlayer: {gold: 0, score: pomelo.app.globalData.players[uid].score}
  };
  cb(null, backMsg);
};

/*
* 玩家离开桌子
* */
tableManager.leaveTable = function(uid, tableId, cb) {
  // 自动存分
  transGoldAuto(uid);
  var backMsg = {
    code: CODE.SUCCESS,
    updatePlayer: {gold: pomelo.app.globalData.players[uid].gold, score: 0}
  };

  // 将这个玩家从这个channel中移除
  channelService.remove(uid, pomelo.app.globalData.players[uid].sid, tableId, function(err, isTableEmpty) {
    // 如果桌子不为空，则要通知剩余的玩家有人离开了
    if(!isTableEmpty) {
      var chairId = pomelo.app.globalData.players[uid].chairId;
      pomelo.app.globalData.tables[tableId][chairId] = null;
      channelService.pushByChannel(pomelo.app.globalData.players[uid].tableId, ROUTE.PLAYER_QUIT, {uid: uid, chairId: chairId});
      return cb(null, backMsg);
    }

    // 桌子已经空了，移除桌子实例
    pomelo.app.globalData.removeTable(tableId);
    cb(null, backMsg);
  });
};

/*
* 客户端成功载入游戏场景
* */
tableManager.loadComplete = function(uid) {
  var player = pomelo.app.globalData.players[uid],
      tableId = player.tableId,
      chairId = player.chairId,
      score = player.score,
      lottery = player.lottery,
      timer = pomelo.app.globalData.timers[tableId];

  // 这张桌子没有玩家在玩，则创建新的桌子实例
  if(!timer) {
    pomelo.app.globalData.timers[tableId] = new Table({tableId: tableId}).timer;
    channelService.pushByChannel(player.tableId, ROUTE.NEW_ENTER, {playerList: [{uid: uid, enterTime: 0, chairId: chairId, score: score, lottery: lottery}]});
    return;
  }

  // 已有玩家在玩
  // 记录进入桌子的时间
  var nowTime = Date.now() - pomelo.app.globalData.tableInitTime[tableId];
  pomelo.app.globalData.tables[tableId][chairId].enterTime = nowTime;

  // 通知客户端该桌子当前出鱼状态，组成json数组(fishList)的形式通知客户端
  var fishData = pomelo.app.globalData.fishs[tableId],
      fishList = [];
  for(var i in fishData) {fishList.push(fishData[i]);}

  channelService.pushToPlayer(uid, ROUTE.FISH_STATUS, {fishList: fishList, currentTime: nowTime});

  // 通知桌子上的玩家有新玩家加入以及最新的桌子信息
  var tableData = pomelo.app.globalData.tables[tableId],
      playerList = [],
      leftData = {},
      rightData = {};

  leftData.uid = tableData.left.uid;
  leftData.enterTime = tableData.left.enterTime;
  leftData.chairId = CONSTS.CHAIR.LEFT;
  rightData.uid = tableData.right.uid;
  rightData.enterTime = tableData.right.enterTime;
  rightData.chairId = CONSTS.CHAIR.RIGHT;
  playerList.push(leftData);
  playerList.push(rightData);

  channelService.pushByChannel(player.tableId, ROUTE.NEW_ENTER, {playerList: playerList});
};

/*
* 快速开始游戏
* */
tableManager.quickStart = function(uid, mode, cb) {
  var tableData = pomelo.app.globalData.tables,                         //当前模式桌子状态数据
      tableId,                                                          //桌子id
      chairId,                                                          //座位id
      tableIdList = [];                                                 //存放桌子id的数组

  // 验证字段有效性
  if(!tableData) {return cb(new Error(CONSTS.ERR.LOGIC_ERR), {code: CODE.DATA_ERROR});}

  // 遍历桌子状态数据，若有真实玩家存在的桌子，则选择座位坐下
  for(var i in tableData) {
    var data = tableData[i];

    // 记录为空的桌子id
    if(!data.left && !data.right) {
      tableIdList.push(data.tableId);
    }

    // 真实玩家占据了一个座位的桌子，优先级最高，让玩家在此坐下
    if((data.left && !data.right) || (!data.left && data.right)) {
      tableId = data.tableId;
      chairId = data.left ? CONSTS.CHAIR.RIGHT : CONSTS.CHAIR.LEFT;
      this.sitDown(uid, tableId, chairId, cb);
      return;
    }
  }

  // 遍历后未发现有玩家存在的桌子，则选择桌子号靠前的坐下
  if(!tableId || !chairId) {
    tableId = tableIdList.shift();
    chairId = Math.random() > 0.5 ? CONSTS.CHAIR.LEFT : CONSTS.CHAIR.RIGHT;
  }

  this.sitDown(uid, tableId, chairId, cb);
};

/*
* 获取桌子上另一个玩家
* */
tableManager.getOtherPlayer = function(uid) {
  var player = pomelo.app.globalData.players[uid],
      tableId = player.tableId,
      chairId = player.chairId,
      table = pomelo.app.globalData.tables[tableId],
      otherChairId = chairId == CONSTS.CHAIR.LEFT ? CONSTS.CHAIR.RIGHT : CONSTS.CHAIR.LEFT;

  return table[otherChairId];
};

/*
* 自动存分函数  分值 --> 金币
* 只在桌子上使用，不对外暴露
* */
function transGoldAuto(uid) {
  var player = pomelo.app.globalData.players[uid];
  player.gold += Math.round(player.score / CONSTS.TRANS_RATE);
  player.score = 0;
}

/*
* 自动取分函数  金币 --> 分值
* 只在桌子上使用，不对外暴露
* */
function transScoreAuto(uid) {
  var player = pomelo.app.globalData.players[uid];
  player.score = player.gold * CONSTS.TRANS_RATE;
  player.gold = 0;
}