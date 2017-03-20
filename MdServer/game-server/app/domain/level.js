
var level = function (opts) {
    if(opts) {
        this.levels = opts.split(",");
    }else {
        this.levels = [];
    }
};

module.exports = level;

level.prototype.unlock = function(star) {
    if(star) {
        this.levels.push(star);
    }
};

level.prototype.UpdateStar = function(level, star) {
    if(level >= this.levels.length || level < 0)
    {
        return;
    }
    var oldValue = this.level[level];
    star > oldValue ? this.levels[level] = star : 0;

    if(level+1 == this.levels.length)
    {
        this.levels.push(1);
    }
};

level.prototype.toString = function() {
    return this.levels.toString();
};
