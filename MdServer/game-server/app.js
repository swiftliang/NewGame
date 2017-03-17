var pomelo = require('pomelo'),
    mysqlConfig = require('./config/mysql.json'),
    sqlClient = require('./app/dao/mysql/mysql.js');
var sync = require('pomelo-sync-plugin');
var PlayerManager = require('./app/domain/PlayerManager');
/**
 * Init app for client.
 */
var app = pomelo.createApp();
app.set('name', 'MdServer');

app.configure('production|development', function() {

  app.before(pomelo.filters.toobusy());
  // proxy configures
  app.set('proxyConfig', {
    cacheMsg: true,
    interval: 30,
    lazyConnection: true
    // enableRpcLog: true
  });

  // remote configures
  app.set('remoteConfig', {
    cacheMsg: true,
    interval: 30
  });

  //app.route('connector', routeUtil.connector);

  //app.loadConfig('mysql', app.getBase() + '/../shared/config/mysql.json');

  app.filter(pomelo.filters.timeout());
});

app.configure('production|development', 'auth', function() {
  // load session congfigures
  //app.set('session', require('./config/session.json'));
});


app.configure('production|development', 'auth|connector|master', function() {

  var dbclient = new sqlClient(mysqlConfig);
  app.set('dbclient', dbclient);

  app.use(sync, {sync: {path:__dirname + '/app/dao/mapping', dbclient: dbclient}});
});


// app configuration
app.configure('production|development', 'connector', function(){
  app.set('connectorConfig',
    {
      connector : pomelo.connectors.hybridconnector,
      heartbeat : 3,
      useDict : true,
      useProtobuf : true
    });

  app.set('PlayerManager', new PlayerManager());
});


app.configure('production|development', 'gate', function(){
  app.set('connectorConfig',
      {
        connector : pomelo.connectors.hybridconnector,
        useProtobuf : true
      });
});
// start app
app.start();

process.on('uncaughtException', function (err) {
  console.error(' Caught exception: ' + err.stack);
});
