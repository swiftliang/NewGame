
var EventEmitter = require('event').EventEmitter;
var util = require('util');

var Persistent = function(opts) {
    EventEmitter.call(this);
};

util.inherits(Persistent, EventEmitter);

module.exports = Persistent;

Persistent.prototype.coin = function() {
    this.emit('coin');
};

Persistent.prototype.level = function() {
    this.emit('level');
};