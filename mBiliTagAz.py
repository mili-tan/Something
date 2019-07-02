#------------------------------------------------------------------------------------------------------------------------
# * Copyright (c) 2017-2019 INARI. All Rights Reserved.
# * 
# * This code is licensed under the MIT License.
# *
# * Author : Milkey
# * Version: 0.3
#------------------------------------------------------------------------------------------------------------------------

import json,time,openpyxl
import requests
from openpyxl import Workbook

tagId = 1
page = 1

req = requests.get('https://api.bilibili.com/x/tag/detail?pn={}&ps=40&tag_id={}'.format(page,tagId))

jStr = str(req.content,'utf-8')
jObj = json.loads(jStr)
resObj = jObj['data']['news']['archives']
tagName = jObj['data']['info']['tag_name']

wBook = Workbook()
wSheet = wBook.active
wSheet.append(['AV号', '标题', '简介', 'up主', '上传时间', '视频时长', ' ', '播放', '弹幕', '回复', '收藏', '硬币' ,' ', '动态标签'])

for r in resObj:
    avNum = 'av' + str(r["aid"])
    title = r["title"]
    desc = r["desc"]
    upTime = time.strftime("%Y-%m-%d %H:%M:%S",time.localtime(r["ctime"]))
    upUser = r['owner']['name']
    duration = time.strftime("%H:%M:%S", time.gmtime(float(r["duration"])))
    
    statObj = r['stat']
    view = statObj['view']
    danmaku = statObj['danmaku']
    reply = statObj['reply']
    favorite = statObj['favorite']
    coin = statObj['coin']

    tag = r['dynamic']
    wSheet.append([avNum, title, desc, upUser, upTime, duration, ' ', view, danmaku, reply, favorite, coin ,' ', tag])

wBook.save("{}-{}-{}.xlsx".format(tagName,str(time.strftime("%Y-%m-%d %H-%M-%S", time.localtime())),page))
    
