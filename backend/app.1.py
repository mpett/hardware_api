#!flask/bin/python
from flask import Flask, jsonify, abort, make_response, request, url_for

app = Flask(__name__)

hardware_list = [
    {
        'id': 1,
        'name': u'GTX Titan',
        'platform': u'PC', 
        'ip': u'nVIDIA', 
        'leased': True
    },
    {
        'id': 2,
        'name': u'1 TB Hard Drive',
        'platform': u'PS4',
        'ip': u'Seagate',  
        'leased': False
    },
    {
        'id': 3,
        'name': u'GTX 1080',
        'platform': u'PC', 
        'ip': u'nVIDIA', 
        'leased': True
    }
]

def make_public_hardware(hardware):
    new_hardware = {}
    for field in hardware:
        if field == 'id':
            new_hardware['uri'] = url_for('get_hardware', hardware_id=hardware['id'], _external=True)
        else:
            new_hardware[field] = hardware[field]
    return new_hardware

@app.route('/todo/api/v1.0/hardware_list', methods=['GET'])
def get_hardware_list():
    return jsonify({'hardware_list' : [make_public_hardware(hardware) for hardware in hardware_list]})

@app.route('/todo/api/v1.0/hardware_list/<int:hardware_id>', methods=['GET'])
def get_hardware(hardware_id):
    hardware = [hardware for hardware in hardware_list if hardware['id'] == hardware_id]
    if len(hardware) == 0:
        abort(404)
    return jsonify({'hardware' : hardware[0]})

@app.route('/todo/api/v1.0/hardware_list/<string:hardware_platform>', methods=['GET'])
def get_hardware_by_platform(hardware_platform):
    hardware = [hardware for hardware in hardware_list if hardware['platform'] == hardware_platform]
    if len(hardware) == 0:
        abort(404)
    return jsonify({'hardware' : hardware})

@app.route('/todo/api/v1.0/active_leases', methods=['GET'])
def get_active_leases():
    hardware = [hardware for hardware in hardware_list if hardware['leased']]
    if len(hardware) == 0:
        abort(404)
    return jsonify({'hardware' : hardware})

@app.route('/todo/api/v1.0/hardware_list', methods=['POST'])
def create_hardware():
    if not request.json or not 'name' in request.json:
        abort(400)
    hardware = {
        'id' : hardware_list[-1]['id'] + 1,
        'name' : request.json['name'],
        'platform' : request.json.get('platform', ""),
        'ip' : request.json.get('ip', ""),
        'leased' : False
    }
    hardware_list.append(hardware)
    return jsonify({'hardware' : hardware}), 201

@app.route('/todo/api/v1.0/hardware_list/<int:hardware_id>', methods=['PUT'])
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
    hardware[0]['name'] = request.json.get('name', hardware[0]['name'])
    hardware[0]['platform'] = request.json.get('platform', hardware[0]['platform'])
    hardware[0]['ip'] = request.json.get('ip', hardware[0]['ip'])
    hardware[0]['leased'] = request.json.get('leased', hardware[0]['leased'])
    return jsonify({'hardware': hardware[0]})

@app.route('/todo/api/v1.0/hardware_list/<int:hardware_id>', methods = ['DELETE'])
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
    app.run(debug=True)