/**
 * 
 * 策划配置表数据管理模块
 */
module.exports = function(app) {
  return new configData(app);
};

var configData = function(app) {
  this.fishOut = [
    {
      interval: 500,
      pathList: [{id: 1, rate: 50}, {id: 2, rate: 50}],
      fishList: [{id: 1, rate: 40}, {id: 2, rate: 60}]
    },
    {
      interval: 1000,
      pathList: [{id: 3, rate: 50}, {id: 4, rate: 50}],
      fishList: [{id: 3, rate: 10}, {id: 4, rate: 90}]
    },
    {
      interval: 1500,
      pathList: [{id: 5, rate: 50}, {id: 6, rate: 50}],
      fishList: [{id: 5, rate: 80}, {id: 6, rate: 20}]
    }
  ];
};