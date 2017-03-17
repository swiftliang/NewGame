/**
 *
 */
var Pool = require('generic-pool');

var sqlClient = function( config ){
	this.mysqlPool = Pool.Pool({
			name: 'mysql',
			create: function(callback){
				var mysql = require('mysql');
				var client = mysql.createConnection({
					host :  config.host,
					port :  config.port,
					user  :  config.user,
					password : config.password,
					database : config.database,
					charset : config.charset,
					debug : false,
					multipleStatements: true
				});
				callback(null, client);
			},
			destroy: function(client){
				client.end();
			},
			max: config.connectNum,
			idleTimeoutMillis: 30000,
			log: false
		}
	);
};

sqlClient.prototype.query = function(sql, args, cb){
	var pool = this.mysqlPool;
	pool.acquire(function(err, client){
		if (!!err){
			// 获取连接失败
			console.error('[sqlqueryErr]' + err.stack );
			return;
		}
		client.query(sql, args, function(err, res){
			pool.release(client);
			cb(err, res);
		});
	});
};

sqlClient.prototype.shutdown = function(app){
	this.mysqlPool.destroyAllNow();
};

module.exports = sqlClient;