// 云函数入口文件
const cloud = require('wx-server-sdk')
// 使用当前云环境
cloud.init({ env: cloud.DYNAMIC_CURRENT_ENV }) 

//获取数据库的引用实例，以便操作数据
const db = cloud.database();
//获取集合，根据关卡进行逆向排序
const gamedata = db.collection('UserData').orderBy('gamedata.Score', 'desc');//可以通过一层一层获取
const MAX_LIMIT = 50

// 云函数入口函数
exports.main = async (event, context) => {
  const wxContext = cloud.getWXContext()

  //直接取出50条（最多一次性取出100条）
  const results = await gamedata.limit(MAX_LIMIT).get()

  // 构造 GameRankDatas 对象
  let gameRankDatas = [];

  // 遍历每个 resolved promise 的结果
  for (let i = 0; i < results.data.length; i++) {
    // 将结果添加到 rankDatas 数组中
    let score = results.data[i].gamedata.Score;
    if (score > 0) {
      let avatorURL = results.data[i].gamedata.HeadUrl;
      let userName = results.data[i].gamedata.Name;
      
      let openid  = results.data[i].openid;
      gameRankDatas.push({OpenID:openid, Head: avatorURL, Name: userName, Score: score});
    }
  }

  return {
    data: gameRankDatas
  };
}