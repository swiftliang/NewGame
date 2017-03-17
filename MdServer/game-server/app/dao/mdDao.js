var pomelo = require('pomelo');
var utils = require('../util/utils');
var MDGame = require('../domain/MDGame');

var mdDao = module.exports;

mdDao.getGameInfoByuId = function(userId, cb) {
    var sql = 'select * from MDGame where uid = ?';
    var args = [userId];

    pomelo.app.get('dbclient').query(sql, args, function(err, res){
        if(err !== null) {
            utils.invokeCallback(cb, err.message, null);
        } else {
            //var userId = 0;
            if(!!res && res.length >= 1) {
                var player = createPlayer(res[0]);
                utils.invokeCallback(cb, null, null, player);
            }
            else {
                CreateGameInfo(userId, cb);
                //utils.invokeCallback(cb, null, []);
            }
        }
    });
};

var CreateGameInfo = function(userId, cb) {
    var sql = 'insert into MDGame (uid, coin, stars) value (?, ?, ?)';
    var args = [userId, 100, 0];

    pomelo.app.get('dbclient').query(sql, args, function(err, res) {
        if(err) {
            utils.invokeCallback(cb, err, null);
        } else {
            var player = createPlayer(res[0]);
            utils.invokeCallback(cb, null, player);
        }
    });
};

var createPlayer = function(MdGame) {
    var player = new MDGame(MdGame);
    var app = pomelo.app;
    player.on('coin', function() {
        app.get('sync').exec('coinSync.updateCoin', player.uid, player);
    });
    player.on('level', function() {
        app.get('sync').exec('starSync.updateCoin', player.uid, player);
    });
    return player;
};