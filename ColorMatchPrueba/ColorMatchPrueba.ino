#include <Servo.h>
#include <MD_TCS230.h>
#include <FreqCount.h>
#include "ColorMatch.h"

Servo myservo;

// Pines del L298N
#define m1 11
#define m2 12
#define pwm 10

// Pines del TCS230
#define S0 7
#define S1 4
#define S2 2
#define S3 3
#define OE_OUT 8  // LOW = ENABLED

// Inicializar sensor TCS230
MD_TCS230 CS(S2, S3, OE_OUT);

// Variables globales
bool objectDetected = false;
colorData rgb;  
int flag = 0;   
int velocidadBanda = 40; 
bool bandaActiva = false; // empieza detenida

void setup() {
  Serial.begin(9600);

  pinMode(m1, OUTPUT);
  pinMode(m2, OUTPUT);
  pinMode(pwm, OUTPUT);
  
  detenerBanda();  

  myservo.attach(9);
  myservo.write(90);

  pinMode(S0, OUTPUT);
  pinMode(S1, OUTPUT);
  digitalWrite(S0, HIGH);
  digitalWrite(S1, LOW);

  CS.begin();
  CS.setDarkCal(&sdBlack);
  CS.setWhiteCal(&sdWhite);

  Serial.println("Sistema listo. Banda detenida.");
}
void loop() {
  if (Serial.available() > 0) {
    String comando = Serial.readStringUntil('\n');
    comando.trim();

    if (comando == "START") {
      bandaActiva = true;
      Serial.println("Sistema: BANDA_INICIADA"); 
    }
    else if (comando == "STOP") {
      bandaActiva = false;
      Serial.println("Sistema: BANDA_DETENIDA"); 
    }
  }

  if (bandaActiva) {
    iniciarBanda(velocidadBanda);
    leerColor();

    if ((flag == 1 || flag == 2) && !objectDetected) {
      objectDetected = true;
      detenerBanda();
      
      if (flag == 1) {
        Serial.println("AMARILLO"); 
        myservo.write(75);
      }
      else if (flag == 2) {
        Serial.println("VERDE"); 
        myservo.write(180);
      }
      
      delay(800);
      myservo.write(40);
      delay(300);
      objectDetected = false;
      flag = 0;
    }
  } else {
    detenerBanda();
  }

  delay(100);
}

void leerColor() {
  CS.read();
  while (!CS.available()) delay(1);
  CS.getRGB(&rgb);

  uint8_t matchIndex = colorMatch(&rgb);

  if (strcmp(ct[matchIndex].name, "YELLOW") == 0) flag = 1;
  else if (strcmp(ct[matchIndex].name, "GREEN") == 0) flag = 2;
  else flag = 0;
}

uint8_t colorMatch(colorData *rgb) {
  int32_t d;
  uint32_t v, minV = 999999L;
  uint8_t minI = 0;
  const int tolerancia = 12;

  for (uint8_t i = 0; i < ARRAY_SIZE(ct); i++) {
    v = 0;
    for (uint8_t j = 0; j < RGB_SIZE; j++) {
      d = ct[i].rgb.value[j] - rgb->value[j];
      if (abs(d) <= tolerancia) d = 0;
      v += (d * d);
    }
    if (v < minV) {
      minV = v;
      minI = i;
    }
  }

  return minI;
}

void detenerBanda() {
  digitalWrite(m1, LOW);
  digitalWrite(m2, LOW);
  analogWrite(pwm, 0);
}

void iniciarBanda(int velocidad) {
  digitalWrite(m1, HIGH);
  digitalWrite(m2, LOW);
  analogWrite(pwm, velocidad);
}