var pomelo = require('pomelo');
var User = require('../domain/User');
var utils = require('../util/utils')

var userDao = module.exports;

userDao.getUserInfo = function (username, passwd, cb) {
    var sql = 'select * from userinfo where name = ?';
    var args = [username];

    pomelo.app.get('dbclient').query(sql, args, function(err, res) {
        if(err != null) {
            utils.invokeCallback(cb, err, null);
        }else {
            var userId = 0;
            if(!!res && res.length === 1) {
                var rs = res[0];
                userId = rs.id;
                rs.uid = rs.id;
                utils.invokeCallback(cb, null, rs);
            } else {
                utils.invokeCallback(cb, null, {uid: 0, username: username});
            }
        }
    });
};

userDao.getUserByName = function (uername, cb) {
    var sql = 'select * from userinfo where userName = ?';
    var args = [uername];
    pomelo.app.get('dbclient').query(sql, args, function(err, res) {
        if(err != null) {
            utils.invokeCallback(cb, err, null);
        } else {
            if(!!res && res.length === 1) {
                var rs = res[0];
                var user = new User({id: rs, name: rs.name, password: rs.password, from: rs.from});
                utils.invokeCallback(cb, null, user);
            } else {
                utils.invokeCallback(cb, ' user not exist ', null);
            }
        }
    });
};

userDao.getUserById = function(uid, cb) {
    var sql = 'select * from userinfo where id =?';
    var args = [uid];
    pomelo.app.get('dbclient').query(sql, args, function(err, res){
        if(err != null){
            utils.invokeCallback(cb, err, message, null);
            return;
        }

        if(!!res && res.length > 0) {
            utils.invokeCallback(cb, null, new User(res[0]));
        } else {
            utils.invokeCallback(cb, ' user not exist ', null);
        }
    });
};