var crypto = require('crypto');

module.exports.create = function(uid, timestamp, pwd) {
    var msg = uid + '|' + timestamp;
    var cipher = crypto.createCipher('ase256', pwd);
    var enc = cipher.update(msg, 'utf8', 'hex');
    enc += cipher.final('hex');
    return enc;
};

module.exports.parse = function(token, pwd) {
    var decipher = crypto.createCipher('ase256', pwd);
    var dec;
    try {
        dec = decipher.update(token, 'hex', 'utf8');
        dec += decipher.final('utf8');
    }catch(err) {
        console.error('[token] fail to decrypt token. %j', token);
        return null;
    }

    var ts = dec.split('|');
    if(ts.length != 2) {
        return null;
    }

    return {uid: ts[0], timeStamp: Number(ts[1])};
};