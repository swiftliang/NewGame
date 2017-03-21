/*
* connector服务器处理机
* */
var CODE = require('../../../consts/code.js'),
     async = require('async'),
     Player = require('../../../domain/entity/player.js'),
     authManager = require('../../../domain/auth/authManager.js');

module.exports = function(app) {
   return new Handler(app);
};

var Handler = function(app) {
      this.app = app;
};

var handler = Handler.prototype;

/*
 *  客户端请求连接connector的入口，客户端通过'connector.entryHandler.enter'请求
 * */
handler.enter = function(msg, session, next) {
   //console.log('----- connector -----: ' + msg.uid);
   var self = this,
        uid = msg.uid,
        playerData,
        sessionService = self.app.get('sessionService');

   // 使用async.waterfall控制顺序执行
   async.waterfall([
      //// 登录认证
      //function(cb) {
      // authManager.checkLogin('test', '123456', cb);                                //通过authManager模块对登录数据做校验
      //},
    //
      //// 根据认证结果做相应处理
      //function(code, player, cb) {
      // if(code != CODE.SUCCESS) {                                                          //非法登录数据，返回错误，断开连接
      //    next(null, {code: code});
      //    sessionService.kickBySessionId(session.id, 'kick');
      //    return;
      // }
    //
      // playerData = player;                                                                     //获取玩家数据
      // cb(null);
      //},

      // session管理
      function(cb) {
         if(!!sessionService.getByUid(uid)) {                                             //已有此玩家在线，踢掉在线玩家
            sessionService.kick(uid, function() {});
            return next(null, {code: CODE.FAILED});
         }
         session.bind(uid);                                                                      //将uid与session绑定
         session.on('closed', onUserLeave.bind(null, self.app));                   //监听断开session的事件
         cb(null);
      }
   ], function(err, result) {
      var playerData = {uid: uid, nickName: "dev-" + uid, photoId: 1, level: 1, gold: 888, score: 0, lottery: 100};
      next(null, {code: CODE.SUCCESS, playerData: playerData});

      //var player = {uid: uid, sid: self.app.get('serverId')};
      playerData.sid = self.app.get('serverId');
      self.app.rpc.gameFrame.gameRemote.addPlayer(session, new Player(playerData), function() {});
      self.app.logger.info('+++ connect +++: ' + uid);
   });
};

/*
* 监听到玩家离开事件后的操作
* */
function onUserLeave(app, session) {
   if(!session || !session.uid) {return;}

   // 移除玩家缓存数据
   app.rpc.gameFrame.gameRemote.removePlayer(session, session.uid, function() {});

   // 记录玩家退出日志
   app.logger.info('=== socket disconnect ===: ' + session.uid);
}