// 云函数入口文件
const cloud = require('wx-server-sdk');
cloud.init({env:cloud.DYNAMIC_CURRENT_ENV});
 
//获取数据库的引用
const db = cloud.database();
const gamedata = db.collection('UserData');
 
// 云函数入口函数
//上传用户存档数据
exports.main = async (event,context)  => {
   const wxContext = cloud.getWXContext();
 
   //查询用户是否已经保存过数据
   let _isHas = await gamedata.where({
       openid:wxContext.OPENID
   }).get();
 
   //如果没有，首次保存
   if(_isHas.data.length==0){
       let _isAdd = null;
       let addData = {
           openid:wxContext.OPENID,
           gamedata:event,//event.gamedata unity调用直接读event
       }
       _isadd = await gamedata.add({
           data:addData
       })
       return{
           code:0,
           res:_isadd,
           data:addData,
       };
   }
   //如果有数据，则更新
   else{
       return await gamedata.where({
           openid:wxContext.OPENID
       }).update({
           data:{
               openid:wxContext.OPENID,
               gamedata:event,//event.gamedata
           }
       })
   }
};