#include "DHT.h"
#include <WiFi.h>
#include <FirebaseESP32.h>

#define DHTPIN 21     
#define DHTTYPE DHT22 // DHT 22 (AM2302) sensor type

DHT dht(DHTPIN, DHTTYPE);

#define FIREBASE_HOST "https://xamarinfirebase-4fbd5-default-rtdb.firebaseio.com/"
#define FIREBASE_AUTH "2HEJ8l39TxA71Phl3HTawbaHSwUHrt1sfYVkCEre"
#define WIFI_SSID "Jccoux Redmi 10A Sport"
#define WIFI_PASSWORD "venomouscov123"

FirebaseData firebaseData;

const int FAN_PIN = 12;     // Pin connected to the fan relay
const int HEATING_PIN = 13; // Pin connected to the heating element relay

void setup() {
  Serial.begin(115200);
  dht.begin();
  initWiFi();
  Firebase.begin(FIREBASE_HOST, FIREBASE_AUTH);

  pinMode(FAN_PIN, OUTPUT);
  pinMode(HEATING_PIN, OUTPUT);
}

void loop() {
  delay(2000); // Wait for 2 seconds between measurements

  float humidity = dht.readHumidity();
  float temperature = dht.readTemperature();

  if (isnan(humidity) || isnan(temperature)) {
    Serial.println("Failed to read from DHT sensor!");
    return;
  }

  Serial.print("Humidity: ");
  Serial.print(humidity);
  Serial.print(" %\t");
  Serial.print("Temperature: ");
  Serial.print(temperature);
  Serial.println(" °C");

  // Send data to Firebase
  Firebase.setFloat(firebaseData, "/temperature", temperature);
  Firebase.setFloat(firebaseData, "/humidity", humidity);

  if (firebaseData.dataAvailable()) {
    Serial.println("Data sent to Firebase");
  } else {
    Serial.println("Error sending data to Firebase");
  }

  // Check temperature from Firebase and control the heating element
  if (Firebase.getFloat(firebaseData, "/Sensor/-NertDnHzfptLkDmoInb/TemperatureValue")) {
    float temperatureValue = firebaseData.floatData();

    if (temperature < temperatureValue) {
      // Temperature is below the desired value, turn on the heating element
      digitalWrite(HEATING_PIN, HIGH);
      Serial.println("Heating element turned on");
    } else {
      // Temperature is equal to or above the desired value, turn off the heating element
      digitalWrite(HEATING_PIN, LOW);
      Serial.println("Heating element turned off");
    }
  } else {
    Serial.println("Error reading TemperatureValue from Firebase");
  }

  // Check humidity from Firebase and control the fan
  if (Firebase.getFloat(firebaseData, "/Sensor/-NertDnHzfptLkDmoInb/HumidityValue")) {
    float humidityValue = firebaseData.floatData();

    if (humidity > humidityValue) {
      // Humidity is above 45%, turn on the fan
      digitalWrite(FAN_PIN, HIGH);
      Serial.println("Fan turned on");
    } else {
      // Humidity is below or equal to 45%, turn off the fan
      digitalWrite(FAN_PIN, LOW);
      Serial.println("Fan turned off");
    }
  } else {
    Serial.println("Error reading HumidityValue from Firebase");
  }
}

void initWiFi() {
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  Serial.print("Connecting to WiFi...");
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println(WiFi.status());
  }
  Serial.println("Connected to WiFi");
}
