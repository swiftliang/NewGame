/**
 * 
 * 玩家缓存数据处理机，主要提供玩家数据的跨进程调用
 */
var channelService = require('../../../service/channelService.js');

module.exports = function(app) {
  return new Remote(app);
};

var Remote = function(app) {
  this.app = app;
};

var remote = Remote.prototype;

/*
* 增加玩家信息缓存数据
* */
remote.addPlayer = function(player, cb) {
  this.app.globalData.addPlayer(player);
  cb(null, true);
};

/*
* 玩家离开游戏时，移除玩家信息缓存
* */
remote.removePlayer = function(uid, cb) {
  var self = this;
  var player = self.app.globalData.players[uid];
  if(player && player.tableId) {
    self.app.globalData.clearChair(player.tableId, player.chairId);
    channelService.remove(uid, player.sid, player.tableId, function(err, isTableEmpty) {
      if(isTableEmpty) {self.app.globalData.removeTable(player.tableId);}
    });
  }

  self.app.globalData.removePlayer(uid);
  cb(null, true);
};