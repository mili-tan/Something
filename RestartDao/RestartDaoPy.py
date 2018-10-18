import requests

with open("./appid.txt") as file:
    lines = file.readlines();

url = "https://openapi.daocloud.io/v1/apps/{0}/actions/restart";
appid = str(lines[0]).rstrip("\n");
key = str(lines[1]).rstrip("\n");

url = url.format(appid);
headers = {"Authorization": key};
ret = requests.post(url,data = None,headers = headers);

print("-------------------");
print(ret.url);
print(ret.text);
print("-------------------");