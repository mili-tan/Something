#include <Arduino.h>
#include <WiFi.h>
#include <ESPAsyncWebServer.h>
#include <ArduinoJson.h>

IPAddress local_IP(192, 168, 31, 254);//IP地址
IPAddress gateway(192, 168, 31, 1);//IP网关
IPAddress subnet(255, 255, 255, 0);//子网掩码
const char* ssid = "";
const char* password = "";
AsyncWebServer server(80);

void handleIndex(AsyncWebServerRequest *request) //IP
{
  AsyncResponseStream *response = request->beginResponseStream("application/json");
  DynamicJsonDocument json(1024);
  json["status"] = "ok";
  json["ssid"] = WiFi.SSID();
  json["ip"] = WiFi.localIP().toString();
  serializeJson(json, *response);
  request->send(response);
}

void setup() {
  Serial.begin(115200);
  WiFi.mode(WIFI_STA);//WIFI模式
  if (!WiFi.config(local_IP, gateway, subnet)) {
    Serial.println("固定IP配置失败");
  }
  WiFi.begin(ssid, password);
  Serial.println("");

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("连接网络成功 ");
  Serial.println(ssid);
  Serial.print("IP 地址: ");
  Serial.println(WiFi.localIP());
  server.on("/", HTTP_GET, handleIndex);
  DefaultHeaders::Instance().addHeader("Server", "Eris.ESP");
  DefaultHeaders::Instance().addHeader("Access-Control-Allow-Methods", "*");
  DefaultHeaders::Instance().addHeader("Access-Control-Allow-Headers", "*");
  DefaultHeaders::Instance().addHeader("Access-Control-Allow-Origin", "*");

  server.on("/ping/tcp", HTTP_GET, [] (AsyncWebServerRequest * request) {
    if (request->hasArg("ip") && request->hasArg("port")) {
      String ip = request->arg("ip");
      int port = request->arg("port").toInt();
      AsyncResponseStream *response = request->beginResponseStream("application/json");
      DynamicJsonDocument json(1024);
      json["protocol"] = "TCP";
      json["ip"] = ip;

      int a = millis();
      WiFiClient client;
      if (!client.connect(ip.c_str(), port)) {
        json["status"] = false;
        json["latency"] = 0;
        serializeJson(json, *response);
        request->send(response);
        delay(1000);
        return;
      }
      client.stop();
      int b = millis();

      json["status"] = true;
      json["latency"] = b - a;
      serializeJson(json, *response);
      request->send(response);
    } else {
      request->send(500, "text/plain", "Error");
    }
  });

  server.onNotFound([](AsyncWebServerRequest * request) {
    if (request->method() == HTTP_OPTIONS) {
      request->send(200);
    } else {
      request->send(404);
    }
  });
  server.begin();
}

void loop() {
}
