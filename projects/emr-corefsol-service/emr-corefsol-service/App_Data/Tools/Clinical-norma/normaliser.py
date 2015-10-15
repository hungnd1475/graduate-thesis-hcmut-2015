#!/usr/bin/python
#
#   Copyright 2012 Michele Filannino
#
#   gnTEAM, School of Computer Science, University of Manchester.
#   All rights reserved. This program and the accompanying materials
#   are made available under the terms of the GNU General Public License.
#
#   author: Michele Filannino
#   email:  filannim@cs.man.ac.uk
#
#   This work is part of 2012 i2b2 challenge.
#   For further details, see www.cs.man.ac.uk/~filannim/

import codecs
import os
import re
import sys
from clinical_doc_analyser import DocumentAnalyser
from clinical_norma import normalise

def fix_date_notation(date):
        date = date.split("-")
        if len(date) == 3:
            return date[0] + date[1] + date[2]
        else:
            return "ERROR"

def select_utterance_time(text, doc_info):
    utterance_time = ''
    text = text.strip().lower()
    if text.find('operat') >= 0 or text.find('op ') >= 0:
        utterance_time = fix_date_notation(doc_info.operation_date)
    elif text.find('discha') >= 0 or text.find('today') >= 0:
        utterance_time = fix_date_notation(doc_info.discharge_date)
    elif text.find('admiss') >= 0 or text.find('hospital') >= 0 or text.find('hd') >= 0:
        utterance_time = fix_date_notation(doc_info.admission_date)
    elif text.find('transf') >= 0:
        utterance_time = fix_date_notation(doc_info.transfer_date)
    else:
        utterance_time = fix_date_notation(doc_info.admission_date)
    if utterance_time == "ERROR":
        utterance_time = fix_date_notation(doc_info.admission_date)
    return utterance_time

def main():
    # Read external parameters:
    # - path and file:       "/usr/xxx/data/XXX.xml"
    # - temporal expression: "every two days"
    #
    #
    temporal_expression = sys.argv[2]
    path_and_file = sys.argv[1]

    doc_analyser = DocumentAnalyser()
    path = os.path.abspath(os.path.dirname(path_and_file)) + "/"
    filename = os.path.basename(path_and_file)
    clinical_note = doc_analyser.analyse(path, filename, True)
    utterance_time = select_utterance_time(temporal_expression, clinical_note)

    res = normalise(temporal_expression, utterance_time, True)
    print normalise(temporal_expression, utterance_time, True)
    return res

if __name__ == '__main__':
    res = main()
