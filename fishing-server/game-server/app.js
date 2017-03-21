/*
* 游戏服务器主文件
* */
var pomelo = require('pomelo'),
     routeUtil = require('./app/util/routeUtil'),                                        //路由配置
     logger = require('./app/util/logger.js').logger,                                  //日志配置
     globalData = require('./app/domain/data/globalData.js'),                      //全局缓存数据配置
     configData = require('./app/domain/data/configData.js'),                      //配置表数据配置
    //sqlClient = require('./app/dao/mysql/mysql.js'),                                 //mysql连接池配置
     mysqlConfig = require('./config/mysql.json'),                                    //mysql配置
     //socketManager= require('./app/domain/socket/socketManager.js'),              //socket客户端模块配置
     errManager = require('./app/domain/error/errManager.js');                         //错误处理模块

/**
 * 初始化app
 */
var app = pomelo.createApp();
app.set('name', 'fishing-server');                               //街机钓鱼服务器
app.set('systemMonitor', true);
/*
* 配置connector服务器
* */
app.configure('production|development', 'connector', function(){
   app.set('connectorConfig',
      {
         connector : pomelo.connectors.hybridconnector,
         heartbeat : 500,
         //useDict : true,
         useProtobuf : false
      });
});

/*
* 配置gate服务器
* */
app.configure('production|development', 'gate', function(){
   app.set('connectorConfig',
      {
         connector : pomelo.connectors.hybridconnector,
         useProtobuf : false
      });
});

/*
* 配置gameFrame服务器
* */
app.configure('production|development', 'gameFrame', function(){
   app.set('globalData', new globalData(app), true);                                            //内存数据管理模块
   app.set('configData', new configData(app), true);                                            //配置数据管理模块
   //app.set('socketManager', new socketManager(app));                                            //与大厅服务器交互模块
});

/*
* 配置所有服务器
* */
app.configure('production|development', function() {
   // 针对chat服务器的路由,多台chat服务器的情况下，前端服务器选择哪台chat服务器来服务
   app.route('chat', routeUtil.chat);

   // 延时过滤器配置
   app.filter(pomelo.timeout());

   // rpc超时配置
   app.set('proxyConfig', {
      timeout: 5 * 60 * 1000
   });

   // 日志模块配置
   app.set('logger', logger, true);

   // 错误处理机
   app.set('errorHandler', errManager);

   // pomelo-sync数据持久化配置
  // var dbClient = new sqlClient(mysqlConfig);
   //app.set('dbClient', dbClient);
   //app.use(sync, {sync: {path: __dirname + '/app/dao/mapping', dbclient: dbClient, interval: 5*1000}});
});

// 启动服务器app主入口
app.start();

process.on('uncaughtException', function(err) {
   console.error(' Caught exception: ' + err.stack);
});