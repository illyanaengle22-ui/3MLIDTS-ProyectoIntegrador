import serial
import mysql.connector
import time
import socket
import threading
import os

arduino = serial.Serial('COM7', 9600, timeout=1)
time.sleep(2)

#conexion
conexion = mysql.connector.connect(
    host="localhost",
    user="root",
    password="",
    database="limones",
    port=3307
)
cursor = conexion.cursor()

def obtener_sesion_actual():
    ruta_sesion = os.path.join(os.path.dirname(__file__), '..', 'sesion_actual.txt')
    with open(ruta_sesion, "r") as f:
        return int(f.read().strip())


comando_actual = ""


def servidor_comandos():
    global comando_actual
    servidor = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    servidor.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    servidor.bind(('127.0.0.1', 5000))
    servidor.listen(5)

    
    while True:
        try:
            cliente, addr = servidor.accept()
            data = cliente.recv(1024).decode('utf-8').strip()
            if data in ['START', 'STOP']:
                comando_actual = data
                print(f" Comando recibido desde web: {data}")
                cliente.send(b"OK")
            cliente.close()
        except Exception as e:
            print(f"⚠️ Error en servidor: {e}")


hilo_servidor = threading.Thread(target=servidor_comandos, daemon=True)
hilo_servidor.start()

print("Sistema iniciado. Esperando comandos...")
ultimo_comando = ""
id_sesion = obtener_sesion_actual()

try:
    while True:
        # Enviar a Arduino
        if comando_actual and comando_actual != ultimo_comando:
            print(f" Enviando '{comando_actual}' al Arduino...")
            arduino.write(f"{comando_actual}\n".encode())
            ultimo_comando = comando_actual
            time.sleep(0.5)
        
        # Leer respuestas 
        if arduino.in_waiting > 0:
            linea = arduino.readline().decode('utf-8').strip()
            print(f"Arduino dice: {linea}")
            
            if "AMARILLO" in linea.upper():
                cursor.execute("UPDATE sesiones SET amarillos = amarillos + 1 WHERE id_sesion = %s", (id_sesion,))
                conexion.commit()
                print("+1 limón AMARILLO")
            elif "VERDE" in linea.upper():
                cursor.execute("UPDATE sesiones SET verdes = verdes + 1 WHERE id_sesion = %s", (id_sesion,))
                conexion.commit()
                print(" +1 limón VERDE")
        
        time.sleep(0.1)

except KeyboardInterrupt:
    print("\n Deteniendo sistema...")
    cursor.execute("UPDATE sesiones SET fecha_fin = NOW() WHERE id_sesion = %s", (id_sesion,))
    conexion.commit()
    arduino.close()
    conexion.close()