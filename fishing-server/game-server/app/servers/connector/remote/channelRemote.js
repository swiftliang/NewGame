/**
 * Created by baoyan on 2016/11/9 0009.
 * channel相关的各种推送、频道逻辑
 */
module.exports = function(app) {
  return new Remote(app);
};

var Remote = function(app) {
  this.app = app;
  this.channelService = app.get('channelService');
};

var remote = Remote.prototype;

/*
* 把玩家加入channel
* */
remote.add = function(uid, sid, rid, cb) {
  this.channelService.getChannel(rid, true).add(uid, sid);
  cb(null, true);
};

/*
* 把玩家从channel移除
* */
remote.remove = function(uid, sid, rid, cb) {
  var channel = this.channelService.getChannel(rid, true);
  channel.leave(uid, sid);

  if(!channel.getMembers().length) {
    return cb(null, true);
  }

  cb(null, false);
};

/*
 * 根据channel进行推送
 * */
remote.pushByChannel = function(rid, route, param, cb) {
  this.channelService.getChannel(rid, false).pushMessage(route, param);
  cb(null, true);
};

/*
* 推送给某个具体的玩家
* */
remote.pushToPlayer = function(uids, route, msg, cb) {
  this.channelService.pushMessageByUids(route, msg, uids, errHandler);
  cb(null, true);
};

// 推送事件错误处理机
function errHandler(err, fails) {
  if(err) {
    pomelo.app.logger.error('push message error: ' + err);
  }
}