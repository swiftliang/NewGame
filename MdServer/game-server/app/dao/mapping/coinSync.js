module.exports = {
    updateCoin:function(dbclient, gameInfo, cb) {
        var sql = 'update MdGame set coin = ?, cointime = ? where id = ?';
        var args = [gameInfo.coin, gameInfo.uid, Data.now()];

        dbclient.query(sql, args, function(err, res) {
            if(err != null) {
                console.error('write mysql failed!' + sql + ' ' + JSON.stringify(gameInfo) + ' stack:' + err.stack);
            }
            if(!!cb && typeof cb == 'function') {
                cb(!!err);
            }
        });
    }
};