#include <WiFi.h>
#include <WiFiClient.h>
#include <WiFiAP.h>
#include <DNSServer.h>
#include <SPIFFS.h>
#include <ESPAsyncWebServer.h>

const char *ssid = "🦉Fukuro测试网络2🦉";

const int ledPin = 2;
String ledState;

// Create AsyncWebServer object on port 80
AsyncWebServer server (80);
DNSServer dnsServer;

// Replaces placeholder with LED state value
String processor(const String& var) {
  Serial.println(var);
  if (var == "STATE") {
    if (digitalRead(ledPin)) {
      ledState = "ON";
    }
    else {
      ledState = "OFF";
    }
    Serial.print(ledState);
    return ledState;
  }
  return String();
}

//class CaptiveRequestHandler : public AsyncWebHandler {
//public:
//  CaptiveRequestHandler() {}
//  virtual ~CaptiveRequestHandler() {}
//  bool canHandle(AsyncWebServerRequest *request){
//    request->addInterestingHeader("/");
//    //return true;
//  }
//  void handleRequest(AsyncWebServerRequest *request) {
//    AsyncResponseStream *response = request->beginResponseStream("text/html");
//    response->print("<!DOCTYPE html><html><head><title>Captive Portal</title></head><body>");
//    response->print("<p>This is out captive portal front page.</p>");
//    response->printf("<p>You were trying to reach: http://%s%s</p>", request->host().c_str(), request->url().c_str());
//    response->printf("<p>Try opening <a href='http://%s/index'>this link</a> instead</p>", WiFi.softAPIP().toString().c_str());
//    response->print("</body></html>");
//    request->send(response);
//  }
//};

void setup() {
  // Serial port for debugging purposes
  Serial.begin(115200);
  pinMode(ledPin, OUTPUT);

  // Initialize SPIFFS
  if (!SPIFFS.begin(true)) {
    Serial.println("An Error has occurred while mounting SPIFFS");
    return;
  }

  WiFi.softAP(ssid);
  IPAddress myIP = WiFi.softAPIP();
  Serial.print("AP IP address: ");
  Serial.println(myIP);

  //server.addHandler(new CaptiveRequestHandler()).setFilter(ON_AP_FILTER);

  server.onNotFound([](AsyncWebServerRequest *request){
  request->redirect("/");;
  });
  
//  server.on("/generate_204", HTTP_GET, [](AsyncWebServerRequest * request) {
//    request->send(200, "text/plain", "Hello World!");
//  });
//
//  server.on("/", HTTP_GET, [](AsyncWebServerRequest * request) {
//    request->send(200, "text/plain", "Hello World!");
//  });

  server.on("/", HTTP_GET, [](AsyncWebServerRequest * request) {
    request->send(SPIFFS, "/index.html", String(), false, processor);
  });

  // Route to load style.css file
  server.on("/style.css", HTTP_GET, [](AsyncWebServerRequest * request) {
    request->send(SPIFFS, "/style.css", "text/css");
  });

  // Route to set GPIO to HIGH
  server.on("/on", HTTP_GET, [](AsyncWebServerRequest * request) {
    digitalWrite(ledPin, HIGH);
    request->send(SPIFFS, "/index.html", String(), false, processor);
  });

  // Route to set GPIO to LOW
  server.on("/off", HTTP_GET, [](AsyncWebServerRequest * request) {
    digitalWrite(ledPin, LOW);
    request->send(SPIFFS, "/index.html", String(), false, processor);
  });

  // Start server
  server.begin();
  dnsServer.start(53, "*", myIP);
}

void loop() {
  dnsServer.processNextRequest();
}
