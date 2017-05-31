'use strict'

import React from 'react';
import {
  Image,
  TouchableOpacity
} from 'react-native';

export default class CancleBtn extends React.Component{
  render() {
    return (
      <TouchableOpacity onPress={this.props.onPress}>
        <Image
          source={require('../../../assets/img/multiply.png')}
          style={[{ width: 18, height: 18, marginLeft: 18, marginTop: 15},]}/>
      </TouchableOpacity>
    );
  }
}
