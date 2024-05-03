// 云函数入口文件
const cloud = require('wx-server-sdk');
cloud.init({env:cloud.DYNAMIC_CURRENT_ENV});
 
//获取数据库的引用
const db = cloud.database();
const gamedata = db.collection('UserData');
 
// 云函数入口函数
//获取用户存档数据
exports.main = async (event,context)  => {
   const wxContext = cloud.getWXContext();
 
   //查询用户是否已经保存过数据
   let data = await gamedata.where({
       openid:wxContext.OPENID
   }).get();
   
   if(data.data.length==0){
    return{
        code:0
    };
   }
   else{
    return {
      data:data.data[0].gamedata
    };
   }
};