#!flask/bin/python
from flask import Flask, jsonify, abort, make_response, request, url_for

app = Flask(__name__)

hardware_list = [
    {
        'id': 1,
        'title': u'Buy groceries',
        'description': u'Milk, Cheese, Pizza, Fruit, Tylenol', 
        'leased': False
    },
    {
        'id': 2,
        'title': u'Learn Python',
        'description': u'Need to find a good Python tutorial on the web', 
        'leased': False
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

@app.route('/todo/api/v1.0/hardware_list', methods=['POST'])
def create_hardware():
    if not request.json or not 'title' in request.json:
        abort(400)
    hardware = {
        'id' : hardware_list[-1]['id'] + 1,
        'title' : request.json['title'],
        'description' : request.json.get('description', ""),
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
    if 'title' in request.json and type(request.json['title']) != unicode:
        abort(400)
    if 'description' in request.json and type(request.json['description']) is not unicode:
        abort(400)
    if 'leased' in request.json and type(request.json['leased']) is not bool:
        abort(400)
    hardware[0]['title'] = request.json.get('title', hardware[0]['title'])
    hardware[0]['description'] = request.json.get('description', hardware[0]['description'])
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