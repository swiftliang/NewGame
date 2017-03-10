var pomelo = require('pomelo');
var utils = require('../util/utils');

var mdDao = module.exports;

mdDao.getGameInfoByuId = function(userId, cb) {
    var sql = 'select * from MDGame where uid = ?';
    var args = [userId];

    pomelo.app.get('dbclient').query(sql, args, function(err, res){
        if(err !== null) {
            utils.invokeCallback(cb, err, null);
        } else {
            var userId = 0;
            if(!!res && res.length === 1) {

            }
        }
    });
};