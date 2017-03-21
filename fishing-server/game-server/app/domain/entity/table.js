/**
 * 
 * 桌子对象
 */
var pomelo = require('pomelo'),
    CONSTS = require('../../consts/consts.js'),
    ROUTE = require('../../consts/route.js'),
    fishOut = require('../table/fishOut.js'),
    channelService = require('../../service/channelService.js'),
    fishOutConfig = pomelo.app.configData.fishOut;

module.exports = function(tableData) {
  return new table(tableData);
};

var table = function(tableData) {
  this.tableId = tableData.tableId;                                                                         //桌子id
  this.fishIdPool = [];                                                                                     //鱼池的鱼的id列表
  for(var i = 1; i <= CONSTS.MAX_FISH_NUM; i++) {
    this.fishIdPool.push(i);                                                                                //初始化鱼池可用鱼的id
  }
  this.fishOut = new fishOut();                                                                             //出鱼算法
  this.fishOutInterval = CONSTS.INTERVAL.ADD_FISH;                                                          //出鱼时间间隔
  this.clearFishInterval = CONSTS.INTERVAL.CLEAR_FISH;                                                      //清理过期鱼时间间隔
  pomelo.app.globalData.tableInitTime[this.tableId] = Date.now();                                           //缓存桌子创建时间
  this.timer = setInterval(onInterval, CONSTS.INTERVAL.SYNC_TIME, this, CONSTS.INTERVAL.SYNC_TIME);         //开启桌子定时器
};

/*
* 桌子上定时执行的逻辑
* */
function onInterval(self, time) {
  // 当前相对时间(桌子开启时间为原点)
  var nowTime = Date.now() - pomelo.app.globalData.tableInitTime[self.tableId];

  // 出鱼
  self.fishOutInterval -= time;
  if(self.fishOutInterval <= 0) {
    addFish(self, nowTime);
    self.fishOutInterval = CONSTS.INTERVAL.ADD_FISH;
  }

  // 清理过期的鱼
  self.clearFishInterval -= time;
  if(self.clearFishInterval <= 0) {
    clearExpireFish(self, nowTime);
    self.clearFishInterval = CONSTS.INTERVAL.CLEAR_FISH;
  }

  // 与客户端同步时间
  syncTimeStamp(self, nowTime);
}

/*
* 定时出鱼
* */
function addFish(self, createTime) {
  // 按照出鱼算法，生成一条/几条鱼
  var fishs = self.fishOut.getFishOut();
  var fishData = pomelo.app.globalData.fishs[self.tableId];

  // 生成的有可能是几条鱼，所以是数组形式
  fishs.map(function(fish) {
    // 增加鱼的生成时间(相对)和id
    fish.createTime= createTime;
    fish.fishId = getFishId(self);

    // 缓存本桌出鱼数据
    if(fishData) {
      fishData[fish.fishId] = fish;
    }else {
      var data = {};
      data[fish.fishId] = fish;
      pomelo.app.globalData.fishs[self.tableId] = data;
    }
  });

  // 推送给本桌的玩家
  channelService.pushByChannel(self.tableId, ROUTE.ADD_FISH, {fishList: fishs});

  //console.log('----- fish out ---------');
  //console.log(fishs);
}

/*
* 定时与客户端同步时间
* */
function syncTimeStamp(self, time) {
  // 推送给本桌玩家当前桌子已创建时间
  channelService.pushByChannel(self.tableId, ROUTE.SYNC_TIME, {currentTime: time});
}

/*
* 定时清除过期的鱼
* */
function clearExpireFish(self, nowTime) {
  var fishData = pomelo.app.globalData.fishs[self.tableId];
  if(!fishData) {return;}

  // 超过了生存时间(expireTime)，清除桌子上这条鱼的数据，释放鱼的id回鱼池(fishIdPool)
  for(var fishId in fishData) {
    var fish = fishData[fishId];
    if(nowTime - fish.createTime >= fish.expireTime) {
      self.fishIdPool.push(fishId);
      pomelo.app.globalData.clearFish(self.tableId, fishId);
    }
  }
}

/*
* 获取鱼的id(唯一)
* */
function getFishId(self) {
  // 返回鱼池数组的第一个id
  return self.fishIdPool.shift();
}


//var table = function(tableData) {
//  this.tableId = tableData.tableId;                                                                         //桌子id
//  this.fishIdPool = [];                                                                                     //鱼池的鱼的id列表
//  for(var i = 1; i <= CONSTS.MAX_FISH_NUM; i++) {
//    this.fishIdPool.push(i);                                                                                //初始化鱼池可用鱼的id
//  }
//  this.fishOut = new fishOut();                                                                             //出鱼算法
//  this.topInterval = fishOutConfig[0].interval;
//  this.middleInterval = fishOutConfig[1].interval;
//  this.bottomInterval = fishOutConfig[2].interval;
//  this.clearFishInterval = CONSTS.INTERVAL.CLEAR_FISH;                                                      //清理过期鱼时间间隔
//  pomelo.app.globalData.tableInitTime[this.tableId] = Date.now();                                           //缓存桌子创建时间
//  this.timer = setInterval(onInterval, CONSTS.INTERVAL.SYNC_TIME, this, CONSTS.INTERVAL.SYNC_TIME);         //开启桌子定时器
//};
//
///*
// * 桌子上定时执行的逻辑
// * */
//function onInterval(self, time) {
//  // 当前相对时间(桌子开启时间为原点)
//  var nowTime = Date.now() - pomelo.app.globalData.tableInitTime[self.tableId];
//  var intervalArr = [];
//
//  self.topInterval -= time;
//  self.middleInterval -= time;
//  self.bottomInterval -= time;
//
//  if(self.topInterval <= 0 || self.middleInterval <= 0 || self.middleInterval <= 0) {
//    intervalArr.push(self.topInterval);
//    intervalArr.push(self.middleInterval);
//    intervalArr.push(self.bottomInterval);
//    addFish(self, nowTime, intervalArr);
//
//    if(self.topInterval <= 0) {
//      self.topInterval = fishOutConfig[0].interval;
//    }
//
//    if(self.middleInterval <= 0) {
//      self.middleInterval = fishOutConfig[1].interval;
//    }
//
//    if(self.bottomInterval <= 0) {
//      self.bottomInterval = fishOutConfig[2].interval;
//    }
//  }
//
//  // 清理过期的鱼
//  self.clearFishInterval -= time;
//  if(self.clearFishInterval <= 0) {
//    clearExpireFish(self, nowTime);
//    self.clearFishInterval = CONSTS.INTERVAL.CLEAR_FISH;
//  }
//}
//
///*
// * 定时出鱼
// * */
//function addFish(self, createTime, intervalArr) {
//  // 按照出鱼算法，生成一条/几条鱼
//  var fishs = self.fishOut.getFishOut(intervalArr);
//  var fishData = pomelo.app.globalData.fishs[self.tableId];
//
//  console.log('----------------------');
//  console.log(fishs);
//  console.log('----------------------');
//
//  // 生成的有可能是几条鱼，所以是数组形式
//  //fishs.map(function(fish) {
//  //  // 增加鱼的生成时间(相对)和id
//  //  fish.createTime= createTime;
//  //  fish.fishId = getFishId(self);
//  //
//  //  // 缓存本桌出鱼数据
//  //  if(fishData) {
//  //    fishData[fish.fishId] = fish;
//  //  }else {
//  //    var data = {};
//  //    data[fish.fishId] = fish;
//  //    pomelo.app.globalData.fishs[self.tableId] = data;
//  //  }
//  //});
//  //
//  //// 推送给本桌的玩家
//  //channelService.pushByChannel(self.tableId, ROUTE.ADD_FISH, {fishList: fishs});
//
//  //console.log('----- fish out ---------');
//  //console.log(fishs);
//}
//
///*
// * 定时清除过期的鱼
// * */
//function clearExpireFish(self, nowTime) {
//  var fishData = pomelo.app.globalData.fishs[self.tableId];
//  if(!fishData) {return;}
//
//  // 超过了生存时间(expireTime)，清除桌子上这条鱼的数据，释放鱼的id回鱼池(fishIdPool)
//  for(var fishId in fishData) {
//    var fish = fishData[fishId];
//    if(nowTime - fish.createTime >= fish.expireTime) {
//      self.fishIdPool.push(fishId);
//      pomelo.app.globalData.clearFish(self.tableId, fishId);
//    }
//  }
//}
//
///*
// * 获取鱼的id(唯一)
// * */
//function getFishId(self) {
//  // 返回鱼池数组的第一个id
//  return self.fishIdPool.shift();
//}