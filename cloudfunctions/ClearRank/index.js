// 云函数入口文件
const cloud = require('wx-server-sdk');
cloud.init({ env: cloud.DYNAMIC_CURRENT_ENV });

// 获取数据库的引用
const db = cloud.database();
const gamedata = db.collection('UserData');

// 云函数入口函数
exports.main = async (event, context) => {
  const wxContext = cloud.getWXContext();

  try {
    // 将 gamedata.Score 大于 0 的记录重置为 0
    const result = await gamedata.where({
      'gamedata.Score': db.command.gt(0)
    }).update({
      data: {
        'gamedata.Score': 0
      }
    });

    return {
      code: 0,
      res: result,
    };
  } catch (err) {
    return {
      code: -1,
      err: err,
    };
  }
};
