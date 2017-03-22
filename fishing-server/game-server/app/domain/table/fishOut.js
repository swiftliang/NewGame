/**
 * 
 * 出鱼的算法
 */
var pomelo = require('pomelo'),
    fishOutConfig = pomelo.app.configData.fishOut;

module.exports = function() {
  return new fishOut();
};

var fishOut = function() {

};

var fishOutPto = fishOut.prototype;

/*
* 出鱼控制逻辑
* */
//fishOutPto.getFishOut = function() {
//  var fishType = Math.ceil(Math.random()*2);                          //鱼的类型
//  var pathId = Math.ceil(Math.random()*5);                            //路径id
//  var expireTime = 10000;                                             //鱼的生存时间
//
//  return [{
//    fisType: fishType,
//    pathId: pathId,
//    expireTime: expireTime
//  }];
//};

/*
* 出鱼控制逻辑
* */
fishOutPto.getFishOut = function() {
  var upperBase = 1000,                                                 //上层出鱼基数
      middleBase = 1000,                                                //中层出鱼基数
      lowerBase = 1000,                                                 //下层出鱼基数
      upperRandom = (Math.random() * 2147483647) % 1000,                //上层随机数(0~999)
      middleRandom = (Math.random() * 2147483647) % 1000,               //中层随机数(0~999)
      lowerRandom = (Math.random() * 2147483647) % 1000,                //下层随机数(0~999)
      fishList = [],                                                    //本次出鱼数据列表
      fishType = 1,                                                     //鱼的类型
      pathId = 1,                                                       //路径id
      expireTime = 10000,                                               //生存时间
      freeTime = 5000,                                                  //逃跑时间
      canCatch = true,                                                  //是否能被捕获
      escapePlace = 40;

  // 上层出鱼数据
  if(upperRandom > (upperBase -= 500)) {
    fishType = 1;
    pathId = Math.ceil(Math.random()*2);
    expireTime = Math.floor(Math.random()*5000 + 3000);
    fishList.push({fishType: fishType, pathId: pathId, expireTime: expireTime, freeTime: freeTime, canCatch: canCatch, escapePlace: escapePlace});
  }else if(upperRandom > (upperBase -= 500)) {
    fishType = 2;
    pathId = Math.ceil(Math.random()*2);
    expireTime = Math.floor(Math.random()*5000 + 3000);
    fishList.push({fishType: fishType, pathId: pathId, expireTime: expireTime, freeTime: freeTime, canCatch: canCatch, escapePlace: escapePlace});
  }

  // 中层出鱼数据
  if(middleRandom > (middleBase -= 800)) {
    fishType = 1;
    pathId = Math.ceil(Math.random()*2 + 2);
    expireTime = Math.floor(Math.random()*5000 + 5000);
    fishList.push({fishType: fishType, pathId: pathId, expireTime: expireTime, freeTime: freeTime, canCatch: canCatch, escapePlace: escapePlace});
  }else if(middleRandom > (middleBase -= 200)) {
    fishType = 2;
    pathId = Math.ceil(Math.random()*2 + 2);
    expireTime = Math.floor(Math.random()*5000 + 5000);
    fishList.push({fishType: fishType, pathId: pathId, expireTime: expireTime, freeTime: freeTime, canCatch: canCatch, escapePlace: escapePlace});
  }

  // 下层出鱼数据
  if(lowerRandom > (lowerBase -= 100)) {
    fishType = 19;
    pathId = 30;
    expireTime = 12000;
    canCatch = false;
    fishList.push({fishType: fishType, pathId: pathId, expireTime: expireTime, freeTime: freeTime, canCatch: canCatch, escapePlace: escapePlace});
  }

  return fishList;
};


/*
* 出鱼控制逻辑
* */
//fishOutPto.getFishOut = function(intervalArr) {
//  var fishList = [];
//
//  console.log(intervalArr);
//  for(var i = 0; i < intervalArr.length; i++) {
//    if(!intervalArr[i]) {
//      var fish = getFish(i);
//      fishList = fishList.concat(fish);
//    }
//  }
//
//  return fishList;
//};

/*
* 根据层级(上、中、下)获取出鱼数据
* 参数： 1、flag  表示上中下层级
* */
function getFish(flag) {
  var base = 100,                                                       //出鱼基数
      fishList = [],                                                    //本次出鱼数据列表
      fishType = 1,                                                     //鱼的类型
      pathId = 1,                                                       //路径id
      expireTime = 5000,                                                //生存时间
      freeTime = 5000,                                                  //逃跑时间
      canCatch = true,                                                  //是否能被捕获
      random = Math.floor((Math.random() * 2147483647)) % 100,                     //基数内的随机值
      config = fishOutConfig[flag],
      fishOutList = config.fishList;

  //fishOutList.map(function(fish) {
  //  if((random < base) && (random > (base -= fish.rate))) {
  //    fishType = fish.id;
  //    fishList.push({fishType: fishType, pathId: pathId, expireTime: expireTime, freeTime: freeTime, random: random});
  //  }
  //});

  console.log(random);

  for(var i = 0; i < fishOutList.length; i++) {
    var fish = fishOutList[i];
    if((random <= base) && (random > (base -= fish.rate))) {
      fishType = fish.id;
      fishList.push({fishType: fishType, pathId: pathId, expireTime: expireTime, freeTime: freeTime, random: random});
      break;
    }
  }
  return fishList;

}