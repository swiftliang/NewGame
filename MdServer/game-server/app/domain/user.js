

var User = function(opts){
    this.uid = opts.id;
    this.name = opts.userName;
    this.from = opts.from || '';
    this.password = opts.password;
    this.lastLoginTime = opts.lastLoginTime;
};

module.exports = User;