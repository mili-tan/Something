#------------------------------------------------------------------------------------------------------------------------
# * Copyright (c) 2017-2019 INARI.DatAz All Rights Reserved.
# * 
# * This code is licensed under the MIT License.
# *
# * Author : Milkey
# * Version: 0.2
#------------------------------------------------------------------------------------------------------------------------

import json,time,openpyxl,math
import requests
from openpyxl import Workbook

favId = 1

preq = requests.get('https://api.bilibili.com/medialist/gateway/base/spaceDetail?media_id={}&pn={}&ps=20&jsonp=jsonp'.format(favId,1))
ppage = math.ceil(json.loads(str(preq.content,'utf-8'))['data']['info']['attr']/20)

wBook = Workbook()
wSheet = wBook.active
wSheet.append(['AV号', '标题', '简介', 'up主', '上传时间', '视频时长', ' ', '播放', '弹幕', '回复', '收藏', '硬币'])

for page in range(1,ppage+1):
    req = requests.get('https://api.bilibili.com/medialist/gateway/base/spaceDetail?media_id={}&pn={}&ps=20&jsonp=jsonp'.format(favId,page))
    jObj = json.loads(str(req.content,'utf-8'))
    resObj = jObj['data']['medias']
    favName = jObj['data']['info']['title']

    for r in resObj:
        avNum = 'av' + str(r["id"])
        title = r["title"]
        desc = r["intro"]
        upTime = time.strftime("%Y-%m-%d %H:%M:%S",time.localtime(r["ctime"]))
        upUser = r['upper']['name']
        duration = time.strftime("%H:%M:%S", time.gmtime(float(r["duration"])))

        statObj = r['cnt_info']
        view = statObj['play']
        danmaku = statObj['danmaku']
        reply = statObj['reply']
        favorite = statObj['collect']
        coin = statObj['coin']

        wSheet.append([avNum, title, desc, upUser, upTime, duration, ' ', view, danmaku, reply, favorite, coin])

wBook.save("mBiliFavAz-{}-{}.xlsx".format(favName,str(time.strftime("%Y-%m-%d %H-%M-%S", time.localtime()))))
    
