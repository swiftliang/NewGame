/**
 * 
 * 游戏逻辑管理模块
 */
var pomelo = require('pomelo'),
    CODE = require('../../consts/code.js'),
    ROUTE = require('../../consts/route.js'),
    CONSTS = require('../../consts/consts.js'),
    channelService = require('../../service/channelService.js'),
    tableManager = require('../table/tableManager.js');

var gameManager = module.exports;

/*
* 收到客户端出钩通知后的处理逻辑
* */
gameManager.shoot = function(uid, angle, cb) {
  // 字段有效性判定
  if(!uid || !angle) {return cb(new Error(CONSTS.ERR.LOGIC_ERR), CODE.DATA_ERROR);}

  var player = pomelo.app.globalData.players[uid];

  // 玩家数据有效性判定
  if(!player || !player.tableId) {return cb(new Error(CONSTS.ERR.LOGIC_ERR), CODE.DATA_ERROR);}

  var s = 10;//桌子倍率
  var consume = s * player.hook;//本次出钩消耗分值
  if((player.score - consume) < 0) {
    return cb(null, {code: CODE.LACK_SCORE});
  }

  player.score -= consume;

  // 本桌广播
  channelService.pushByChannel(player.tableId, ROUTE.SHOOT, {uid: uid, angle: angle, score: player.score});
  cb(null, {code: CODE.SUCCESS});
};

/*
* 客户端碰撞到鱼时的处理逻辑
* */
gameManager.touchFish = function(uid, fishId, cb) {
  // 参数有效性判定
  if(!uid || !fishId) {return cb(new Error(CONSTS.ERR.LOGIC_ERR), {code: CODE.DATA_ERROR});}

  var player = pomelo.app.globalData.players[uid],
      fishs = pomelo.app.globalData.fishs[player.tableId];

  // 鱼池里面没有这条鱼
  if(!fishs || !fishs[fishId]) {
    return cb(new Error(CONSTS.ERR.LOGIC_ERR), {code: CODE.DATA_ERROR});
  }

  // 本桌广播
  channelService.pushByChannel(player.tableId, ROUTE.TOUCH_FISH, {uid: uid, fishId: fishId});
  cb(null, {code: CODE.SUCCESS});
};

/*
* 收到客户端钓鱼结果时的处理逻辑
* */
gameManager.fishResult = function(uid, fishId, isCatch, cb) {
  // 参数有效性判定
  if(!uid || !fishId || typeof isCatch == "undefined") {
    return cb(new Error(CONSTS.ERR.LOGIC_ERR), {code: CODE.DATA_ERROR});
  }

  var player = pomelo.app.globalData.players[uid];
  var s = 10;//桌子倍率
  player.lottery += s * 2;//获得彩票值--桌子倍率乘鱼的分数
  // 本桌广播
  channelService.pushByChannel(player.tableId, ROUTE.FISH_RESULT, {uid: uid, fishId: fishId, isCatch: isCatch, lottery: player.lottery});

  cb(null, {code: CODE.SUCCESS});
};

/*
* 切换鱼钩的处理逻辑
* */
gameManager.changeHook = function(uid, hook, cb) {
  // 参数有效性检测
  if(CONSTS.HOOK.indexOf(hook) === -1 || !pomelo.app.globalData.players[uid].tableId) {
    return cb(new Error(CONSTS.ERR.LOGIC_ERR), {code: CODE.DATA_ERROR});
  }

  pomelo.app.globalData.players[uid].hook = hook;
  var otherPlayer = tableManager.getOtherPlayer(uid);
  // 桌子上还有其他玩家，则推送
  if(!!otherPlayer) {
    channelService.pushToPlayer(otherPlayer.uid, ROUTE.CHANGE_HOOK, {uid: uid, hook: hook});
  }

  cb(null, {code: CODE.SUCCESS});
};