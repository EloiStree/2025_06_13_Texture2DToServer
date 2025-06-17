# pip install opencv-python flask jsonify Pillow pyzbar numpy

import os
import struct
from flask import Flask, request, jsonify
from PIL import Image
import io
import cv2
import numpy as np
from pyzbar.pyzbar import decode

app = Flask(__name__)
import os

# Get the absolute path of the current file
file_path = os.path.abspath(__file__)
directory_path = os.path.dirname(file_path)


def turn_bytes_to_color32_to_image(path_file_to_write,bytes_array_store_color32, width,height):

    # Convert bytes to a numpy array
    image_as_bytes_array = np.frombuffer(bytes_array_store_color32, dtype=np.uint8)
    
    # Reshape the array to the correct dimensions
    image_as_array = image_as_bytes_array.reshape((height, width, 4))  # Assuming 4 channels (RGBA)
    
    # Convert to PIL Image
    image = Image.fromarray(image_as_array, 'RGBA')
    
    # Save the image if a path is provided
    if path_file_to_write:
        image.save(path_file_to_write, format='PNG')
    
    return image


@app.route('/image/get_barcode', methods=['POST'])
def get_barcode():
    image = turn_bytes_to_color32_to_image(request.get_data())

    # Convert PIL Image to OpenCV format
    img_cv = cv2.cvtColor(np.array(image), cv2.COLOR_RGB2BGR)

    # Decode barcodes
    barcodes = decode(img_cv)

    # Extract barcode information
    barcode_data = [barcode.data.decode('utf-8') for barcode in barcodes]

    return jsonify({'barcode': barcode_data})

@app.route('/image/get_qrcode', methods=['POST'])
def get_qrcode():
    image = turn_bytes_to_color32_to_image(request.get_data())

    # Convert PIL Image to OpenCV format
    img_cv = cv2.cvtColor(np.array(image), cv2.COLOR_RGB2BGR)

    # Use OpenCV to detect QR codes
    detector = cv2.QRCodeDetector()
    qrcode_data, _, _ = detector.detectAndDecode(img_cv)

    return jsonify({'qrcode': qrcode_data})


@app.route('/image/get/barcode', methods=['GET'])
def get_barcode_from_last_color32():
    # Read the last saved color32 image
    string_path_of_image = 'image.png'
    path_last_image = os.path.join(directory_path, string_path_of_image)
    image = Image.open(path_last_image)
    decoded_objects = decode(image)
    barcode_data=""
    for obj in decoded_objects:
        barcode_data += f"{obj.type}:{obj.data.decode('utf-8')}\n"
        print("Type:", obj.type)
        print("Data:", obj.data.decode("utf-8"))
    return barcode_data



@app.route('/image/save/color32', methods=['POST'])
def save_image():
    save_color32_image('image')
    return jsonify({'message': 'Image saved successfully'})

def save_color32_image(file_name):
    print("Saving image...")
    bytes_color32 = request.get_data()
    int_little_endian_width = int.from_bytes(bytes_color32[:4], byteorder='little')
    int_little_endian_height = int.from_bytes(bytes_color32[4:8], byteorder='little')
    print(f"Image dimensions: {int_little_endian_width}x{int_little_endian_height}")
    bytes_color32 = bytes_color32[8:]
    megas_bytes = len(bytes_color32) / (1024 * 1024)
    print(f"Image size: {megas_bytes:.2f} MB")

    image_as_bytes_array = np.frombuffer(bytes_color32, dtype=np.uint8)
    pixel_count = image_as_bytes_array.size // 4  # Assuming 4 bytes per pixel (RGBA)
    
    # save image_as_bytes_array as byte file
    string_path_of_image = file_name + '.color32'
    path_bytes_32 = os.path.join(directory_path, string_path_of_image)
    print(f"Saving image to {path_bytes_32}...")
    with open(path_bytes_32, 'wb') as f:
        f.write(bytes_color32)

    path_image = os.path.join(directory_path, file_name+ '.png')
    image = turn_bytes_to_color32_to_image(path_image,bytes_color32,int_little_endian_width,int_little_endian_height)
    # image.save(path_image, format='PNG')

    print(f"Image saved with {pixel_count} pixels.")
    return jsonify({'message': 'Image saved successfully'})


@app.route('/image/get_pixel_count', methods=['POST'])
def get_pixel_count():
    image = turn_bytes_to_color32_to_image(request.get_data())

    # Calculate pixel count
    width, height = image.size
    pixel_count = width * height

    return jsonify({'pixel_count': pixel_count})

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=80)
