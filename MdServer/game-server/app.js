var pomelo = require('pomelo');
var sync = require('pomelo-sync-plugin');
var PlayerManager = require('./app/domain/PlayerManager');
/**
 * Init app for client.
 */
var app = pomelo.createApp();
app.set('name', 'MdServer');

// app configuration
app.configure('production|development', 'connector', function(){
  app.set('connectorConfig',
    {
      connector : pomelo.connectors.hybridconnector,
      heartbeat : 3,
      useDict : true,
      useProtobuf : true
    });

  var dbclient = require('./app/dao/mysql/mysql').init(app);
  app.set('dbclient', dbclient);

  app.loadConfig('mysql', app.getBase() + '/../shared/config/mysql.json');

  app.use(sync, {sync: {path:__dirname + '/app/dao/mapping', dbclient: dbclient}});
});

// start app
app.start();

process.on('uncaughtException', function (err) {
  console.error(' Caught exception: ' + err.stack);
});
