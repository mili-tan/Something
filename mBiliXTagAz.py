import json,time,openpyxl
import requests
from openpyxl import Workbook

tid = 1
page = 1

req = requests.get('https://api.bilibili.com/x/tag/detail?pn={}&ps=40&tag_id={}'.format(page,tid))

jStr = str(req.content,'utf-8')
jObj = json.loads(jStr)
resObj = jObj['data']['news']['archives']

wBook = Workbook()
wSheet = wBook.active

for r in resObj:
    avNum = r["aid"]
    title = r["title"]
    desc = r["desc"]
    upTime = time.strftime("%Y-%m-%d %H:%M:%S",time.localtime(r["ctime"]))
    upUser = r['owner']['name']
    
    statObj = r['stat']
    view = statObj['view']
    danmaku = statObj['danmaku']
    reply = statObj['reply']
    favorite = statObj['favorite']
    coin = statObj['coin']

    tag = r['dynamic']
    wSheet.append([avNum, title, desc, upUser, upTime, ' ', view, danmaku, reply, favorite, coin ,' ', tag])

wBook.save("[]-[].xlsx")
    
