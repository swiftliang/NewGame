/**
 * 
 * channel相关的各种推送、频道管理服务
 */
var pomelo = require('pomelo');

var channelService = module.exports;

// 获取当前channelService
var service = pomelo.app.get('channelService');

///*
//* 将玩家加入某个channel
//* */
//channelService.add = function(uid, sid, channel, chaichannelId) {
//  // 调用remote中add方法将玩家加入channel
//  pomelo.app.rpc.connector.channelRemote.add(pomelo.app.get('servechannelId'), uid, sid, channel, function(err, res) {});
//  // 增加玩家信息中的频道信息缓存
//  pomelo.app.globalData.players[uid].channelId = channel;
//  pomelo.app.globalData.players[uid].chaichannelId = chaichannelId;
//};
//
///*
//* 把玩家移除某个channel
//* */
//channelService.remove = function(uid, sid, channel, cb) {
//  pomelo.app.rpc.connector.channelRemote.remove(pomelo.app.get('servechannelId'), uid, sid, channel, cb);
//};
//
///*
//* 根据channel进行推送
//* */
//channelService.pushByChannel = function(channel, route, param) {
//  pomelo.app.rpc.connector.channelRemote.pushByChannel(pomelo.app.get('servechannelId'), channel, route, param, function(err, res) {});
//};
//
///*
//* 推送给某个具体玩家
//* */
//channelService.pushToPlayer = function(uid, route, msg) {
//  var sid = pomelo.app.globalData.players[uid].sid;
//  var uids = [{uid: uid, sid: sid}];
//
//  pomelo.app.rpc.connector.channelRemote.pushToPlayer(pomelo.app.get('servechannelId'), uids, route, msg, function(err, res) {});
//};

/*
* 将玩家加入某个channel
* */
channelService.add = function(uid, sid, channelId) {
  // 获取channel，没有则创建
  service.getChannel(channelId, true).add(uid, sid);
};

/*
* 将玩家移除某个channel
* 返回值(err, isEmpty)
* */
channelService.remove = function(uid, sid, channelId, cb) {
  // 获取玩家channel，如果存在，移除，如果channel中还有人，返回false
  var channel = service.getChannel(channelId, false);

  if(!channel) {
    return;
  }

  channel.leave(uid, sid);

  if(!channel.getUserAmount()) {
    return cb(null, true);
  }
  //if(!channel.getMembers().length) {
  //  return cb(null, true);
  //}

  cb(null, false);
};

/*
* 根据channel进行推送
* */
channelService.pushByChannel = function(channelId, route, msg) {
  service.getChannel(channelId, false).pushMessage(route, msg);
};

/*
* 推送给某个具体玩家
* */
channelService.pushToPlayer = function(uid, route, msg) {
  var uids = {uid: uid, sid: pomelo.app.globalData.players[uid].sid};
  channelService.pushByUids([uids], route, msg);
};

/*
* 推送给一些指定的uid
* param：Array [{uid: .., sid: ..}]
* */
channelService.pushByUids = function(uids, route, msg) {
  service.pushMessageByUids(route, msg, uids, errHandler);
};

/*
* 错误处理机
* */
function errHandler(err, fails) {
  if(err) {
    pomelo.app.logger.error('push message error: ' + err);
  }
}