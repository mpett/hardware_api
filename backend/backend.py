#!flask/bin/python
import sys
import threading
from flask import Flask, jsonify, abort, make_response, request, url_for
from hardware_db import hardware_list
from threading import Thread
from time import sleep
data_lock = threading.Lock()
lease_thread = threading.Thread()

app = Flask(__name__)

def make_public_hardware(hardware):
    new_hardware = {}
    for field in hardware:
        if field == 'id':
            new_hardware['uri'] = url_for('get_hardware', hardware_id=hardware['id'], _external=True)
        else:
            new_hardware[field] = hardware[field]
    return new_hardware

@app.route('/hardware/api/1.0/hardware_list', methods=['GET'])
def get_hardware_list():
    hardware = [hardware for hardware in hardware_list]
    if len(hardware) == 0:
        abort(404)
    return jsonify(hardware)

@app.route('/hardware/api/1.0/hardware_list/<int:hardware_id>', methods=['GET'])
def get_hardware(hardware_id):
    hardware = [hardware for hardware in hardware_list if hardware['id'] == hardware_id]
    if len(hardware) == 0:
        abort(404)
    return jsonify(hardware[0])

@app.route('/hardware/api/1.0/hardware_list/<string:hardware_platform>', methods=['GET'])
def get_hardware_by_platform(hardware_platform):
    hardware = [hardware for hardware in hardware_list if hardware['platform'] == hardware_platform]
    if len(hardware) == 0:
        abort(404)
    return jsonify(hardware)

@app.route('/hardware/api/1.0/active_leases', methods=['GET'])
def get_active_leases():
    hardware = [hardware for hardware in hardware_list if hardware['leased']]
    if len(hardware) == 0:
        abort(404)
    return jsonify(hardware)

@app.route('/hardware/api/1.0/hardware_list', methods=['POST'])
def create_hardware():
    if not request.json or not 'name' in request.json:
        abort(400)
    hardware = {
        'id' : hardware_list[-1]['id'] + 1 if len(hardware_list) > 0 else 1,
        'name' : request.json['name'],
        'platform' : request.json.get('platform', ""),
        'ip' : request.json.get('ip', ""),
        'leased' : False,
        "time_left_on_lease" : 0
    }
    hardware_list.append(hardware)
    return jsonify(hardware)

@app.route('/hardware/api/1.0/lease/', methods=['PUT'])
def lease_hardware():
    hardware = [hardware for hardware in hardware_list]
    if len(hardware) == 0:
        abort(404)
    if not request.json:
        abort(400)
    if 'name' in request.json and type(request.json['name']) is not str:
        abort(400)
    if 'platform' in request.json and type(request.json['platform']) is not str:
        abort(400)
    if 'leased' in request.json and type(request.json['leased']) is not bool:
        abort(400)
    if 'time_left_on_lease' in request.json and type(request.json['time_left_on_lease']) is not int:
        abort(400)
    name = request.json.get('name', hardware[0]['name'])
    platform = request.json.get('platform', hardware[0]['platform'])
    hardware = list(filter(lambda h : h['name'].lower() == name.lower() and h['platform'].lower() == platform.lower(), hardware))
    if len(hardware) == 0:
        abort(404)
    hardware = list(filter(lambda h : not h['leased'], hardware))
    if len(hardware) == 0:
        return "All items are unavailable."
    hardware[0]['leased'] = True
    time = request.json.get('time_left_on_lease', hardware[0]['time_left_on_lease'])
    global lease_thread
    lease_thread = threading.Timer(0, timer(time, hardware), ()).start()
    return "Successfully leased hardware."

@app.route('/hardware/api/1.0/hardware_list/<int:hardware_id>', methods = ['DELETE'])
def delete_hardware(hardware_id):
    hardware = [hardware for hardware in hardware_list if hardware['id'] == hardware_id]
    if len(hardware) == 0:
        abort(404)
    hardware_list.remove(hardware[0])
    return jsonify({'result' : True})

@app.errorhandler(400)
def not_found_400(error):
    return make_response("Not found.", 400)

@app.errorhandler(404)
def not_found(error):
    return make_response("Not found.", 404)

def timer(time, hardware):
    for i in range(time):
        hardware[0]['time_left_on_lease'] = time-i
        sleep(60)
    hardware[0]['time_left_on_lease'] = 0
    hardware[0]['leased'] = False

if __name__ == '__main__':
    app.run(host="0.0.0.0", port=2204, threaded=True)