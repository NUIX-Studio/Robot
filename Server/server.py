import json
from flask import Flask, request, g

itemsData = {}

filename = 'items.json'


app = Flask(__name__)


@app.route("/tutorial")
def home():
    return "<h1>Tutorial for NUIX Studio<h1>"




@app.route('/api/postitems', methods=['POST'])
def handle_json():
    data = request.json
    global itemsData
    #print(itemsData)
    itemsData = data
    #print(itemsData)
    with open(filename, 'w') as file_object:
        json.dump(itemsData, file_object)
    return data

@app.route('/api/getitems', methods=['GET'])
def handle_json1():
    #print(itemsData)
    return itemsData




@app.route('/api/send', methods=['POST'])
def handle_json2():
    data = request.json
    global itemsData
    #print(itemsData)
    itemsData = data
    print(itemsData)
    with open(filename, 'w') as file_object:
        json.dump(itemsData, file_object)
    return data

@app.route('/api/receive', methods=['GET'])
def handle_json3():
    print(itemsData)
    return itemsData


if (__name__ == "__main__"):
    app.run(host='183.173.113.8', port=5000, debug=True, threaded=False)
