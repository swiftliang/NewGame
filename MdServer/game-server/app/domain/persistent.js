
var EventEmitter = require('event').EventEmitter;
var util = require('util');

var Persistent = function(opts) {
    EventEmitter.call(this);
};

util.inherits(Persistent, EventEmitter);

module.exports = Persistent;

Persistent.prototype.save = function() {
    this.emit('save');
};