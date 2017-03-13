/**
 * Created by Administrator on 2017/3/13 0013.
 */
var PlayerManager =  function() {
    this.players = {};
};

PlayerManager.prototype.add = function(uid, val) {
    var player = this.players[uid];
    if(!!player){
        return;
    }

    this.players[uid] = val;
};

PlayerManager.prototype.remove = function(uid) {
    delete this.players[uid];
};

module.exports = PlayerManager;