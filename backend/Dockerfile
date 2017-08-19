FROM python:3

ADD backend.py /
ADD hardware_db.py /

RUN pip install flask

CMD [ "python", "./backend.py" ]