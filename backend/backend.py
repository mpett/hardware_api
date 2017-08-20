#!flask/bin/python
from flask import Flask, jsonify, abort, make_response, request, url_for
from hardware_db import hardware_list

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
    return jsonify([make_public_hardware(hardware) for hardware in hardware_list])

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
        'id' : hardware_list[-1]['id'] + 1,
        'name' : request.json['name'],
        'platform' : request.json.get('platform', ""),
        'ip' : request.json.get('ip', ""),
        'leased' : False,
        "time_left_on_lease" : 0
    }
    hardware_list.append(hardware)
    return jsonify(hardware), 201

@app.route('/hardware/api/1.0/lease/<int:hardware_id>', methods=['PUT'])
def lease_hardware(hardware_id):
    hardware = [hardware for hardware in hardware_list if hardware['id'] == hardware_id]
    if len(hardware) == 0:
        abort(404)
    if not request.json:
        abort(400)
    if 'leased' in request.json and type(request.json['leased']) is not bool:
        abort(400)
    if 'time_left_on_lease' in request.json and type(request.json['time_left_on_lease']) is not int:
        abort(400)
    hardware[0]['leased'] = request.json.get('leased', hardware[0]['leased'])
    hardware[0]['time_left_on_lease'] = request.json.get('time_left_on_lease', hardware[0]['time_left_on_lease'])
    return jsonify(hardware[0])


@app.route('/hardware/api/1.0/hardware_list/<int:hardware_id>', methods=['PUT'])
def update_hardware(hardware_id):
    hardware = [hardware for hardware in hardware_list if hardware['id'] == hardware_id]
    if len(hardware) == 0:
        abort(404)
    if not request.json:
        abort(400)
    if 'name' in request.json and type(request.json['name']) != unicode:
        abort(400)
    if 'platform' in request.json and type(request.json['platform']) is not unicode:
        abort(400)
    if 'ip' in request.json and type(request.json['ip']) is not unicode:
        abort(400)
    if 'leased' in request.json and type(request.json['leased']) is not bool:
        abort(400)
    if 'time_left_on_lease' in request.json and type(request.json['time_left_on_lease']) is not int:
        abort(400)
    hardware[0]['name'] = request.json.get('name', hardware[0]['name'])
    hardware[0]['platform'] = request.json.get('platform', hardware[0]['platform'])
    hardware[0]['ip'] = request.json.get('ip', hardware[0]['ip'])
    hardware[0]['leased'] = request.json.get('leased', hardware[0]['leased'])
    hardware[0]['time_left_on_lease'] = request.json.get('time_left_on_lease', hardware[0]['time_left_on_lease'])
    return jsonify(hardware[0])

@app.route('/hardware/api/1.0/hardware_list/<int:hardware_id>', methods = ['DELETE'])
def delete_hardware(hardware_id):
    hardware = [hardware for hardware in hardware_list if hardware['id'] == hardware_id]
    if len(hardware) == 0:
        abort(404)
    hardware_list.remove(hardware[0])
    return jsonify({'result' : True})

@app.errorhandler(404)
def not_found(error):
    return make_response(jsonify({'error' : 'Not Found'}), 404)

if __name__ == '__main__':
    app.run(host="0.0.0.0", port=2204)