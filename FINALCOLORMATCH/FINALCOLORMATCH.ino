#include <Wire.h>
#include "Adafruit_TCS34725.h"
#include <Servo.h>

Servo myservo;

// Pines del L298N
const int ENB = 6;  
const int IN3 = 10;
const int IN4 = 11;

int velocidadMotor = 90; 

// Umbral para detectar fruta
const int CLEAR_UMBRAL = 29;

bool frutaDetectada = false;
bool sistemaActivo = true; 

// Sensor
Adafruit_TCS34725 tcs =
  Adafruit_TCS34725(TCS34725_INTEGRATIONTIME_50MS, TCS34725_GAIN_1X);

void setup() {
  Serial.begin(9600);

  myservo.attach(9);
  myservo.write(100);

  pinMode(IN3, OUTPUT);
  pinMode(IN4, OUTPUT);
  pinMode(ENB, OUTPUT); 

  detenerMotor(); 
  sistemaActivo = false; // Movimiento continuo inicial

  if (!tcs.begin()) {
    Serial.println("TCS34725 not found!");
    while (1);
  }
  Serial.println("TCS34725 listo!");
}

void loop() {
   if (Serial.available()) {
    String cmd = Serial.readStringUntil('\n');
    cmd.trim();

    if (cmd == "1") {
      sistemaActivo = true;
      moverDerecha();
      Serial.println(">>> SISTEMA INICIADO");
    }

    if (cmd == "0") {
      sistemaActivo = false;
      detenerMotor();
      Serial.println(">>> SISTEMA DETENIDO");
    }
  }

  // Si el sistema está detenido → NO analizar frutas
  if (!sistemaActivo) return;

  uint16_t r, g, b, c;
  float lux;

  tcs.getRawData(&r, &g, &b, &c);
  lux = tcs.calculateLux(r, g, b);

  Serial.print("R: "); Serial.print(r);
  Serial.print(" G: "); Serial.print(g);
  Serial.print(" B: "); Serial.print(b);
  Serial.print(" C: "); Serial.print(c);
  Serial.print(" LUX: "); Serial.println(lux);


  // DETECTAR FRUTA
  if (!frutaDetectada && c > CLEAR_UMBRAL) {
    Serial.println(">>> FRUTA DETECTADA, DETENIENDO BANDA...");
    frutaDetectada = true;
    detenerMotor();
    delay(300);
  }


  // CLASIFICAR FRUTA
  if (frutaDetectada) {
    int flag = 0;

    if (r >= 15 && r > g && lux > 10) {
      Serial.println("VERDE");
      flag = 1;
    }
    else if (r >= 7 && g<=14 && lux <= 5) {
      Serial.println("ROJO");
      flag = 2;
    }
    else {
      Serial.println("NO CLASIFICADO");
      flag = 0;
    }

    moverDerecha();
    if (flag == 1) {
      myservo.write(145);
      //delay(2000);
      //myservo.write(100);
    }
    else if (flag == 2) {
      myservo.write(60);
      //delay(2000);
      //myservo.write(100);
    }
 //motor
   
    Serial.println(">>> REANUDANDO BANDA...");
    frutaDetectada = false;
  }

  delay(50);
}

//---------------- MOTOR CONTROL ----------------

void moverDerecha() {
  digitalWrite(IN3, HIGH);
  digitalWrite(IN4, LOW);
  analogWrite(ENB, velocidadMotor);  
}

void detenerMotor() {
  digitalWrite(IN3, LOW);
  digitalWrite(IN4, LOW);
  analogWrite(ENB, 0); 
}
