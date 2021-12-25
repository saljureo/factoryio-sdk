# Import BeautifulSoup
from bs4 import BeautifulSoup as bs
content = []
events = ''
states = ''
stateNames = []
transitions = ''
eventLabel = ''
eventId = ''
AddString = ''
initialState = ''
noncontrollableEvents = []
factoryIoFileName = ''
XmlFileName = ''
csFileName = ''
# Read the XML file

factoryIoFileName = 'Machines3AndBuffer3'
XmlFileName = factoryIoFileName + '.xml'
#with open("supervisorBuffer2.xml", "r") as file:
with open(XmlFileName, "r") as file:
    # Read each line in the file, readlines() returns a list of lines
    content = file.readlines()
    # Combine the lines in the list into a string
    content = "".join(content)
    bs_content = bs(content, "lxml") 

events = bs_content.find_all("event")
states = bs_content.find_all("state")
transitions = bs_content.find_all("transition")

for state in states:
    nombre = state["name"]
    try: 
        state["initial"]
        initialState = state["id"]
    except:
        continue
    stateNames.append(nombre)

csFileName = factoryIoFileName + 'Supervisor.cs'
try:
    f = open(csFileName, "x")
except:
    f = open(csFileName, "w")

##############  PROGRAM WRITING STARTS  ########################
#Headers
f.write("using System;\n")
f.write("using System.Collections.Generic;\n")
f.write("using System.Threading;\n\n")

#namespace Controllers - class 
f.write("namespace Controllers.Scenes\n")
f.write("{\n")
stringy = "    class " + factoryIoFileName + "Supervisor\n"
f.write(stringy)
f.write("    {\n\n")
f.write("        // #### VARIABLE CREATION TO ALLOCATE IN MEMORY ####\n")
f.write("        private int currentState;\n")
f.write("        private int evento;\n")
f.write("        private Dictionary<(int, int), int> transiciones;\n")
f.write("        private Dictionary<string, int> eventLabels;\n\n")
f.write("        private Dictionary<int, string> stateLabels;\n\n")
f.write("        public void CreateController()\n")
f.write("        {\n")
f.write("            transiciones = new Dictionary<(int, int), int>();\n")
f.write("            eventLabels = new Dictionary<string, int>();\n")
f.write("            stateLabels = new Dictionary<int, string>();\n\n")

f.write("            currentState = " + initialState + ";\n") 
f.write("            //#########  TRANSICIONES START ############\n\n")


for transition in transitions:
    AddString ='            ' + 'transiciones.Add((' + transition["source"] + ', ' + transition["event"] + '), ' + transition["dest"] + ');\n'
    f.write(AddString)
f.write("\n            //#########  TRANSICIONES END ############\n\n")
f.write("            //#########  EVENTLABEL START ############\n\n")

for event in events:    
    AddString = '            ' + 'eventLabels.Add("' + event["label"] + '", ' + event["id"] + ');\n'
    f.write(AddString)
    try:
        event["controllable"]
        noncontrollableEvents.append(event["id"])
    except:
        continue
f.write("\n            //#########  EVENTLABEL END ############\n\n")
f.write("\n            //#########  STATELABEL START ############\n\n")
for state in states:
    AddString = '            ' + 'stateLabels.Add(' + state["id"] + ', "' + state["name"] + '");\n'
    f.write(AddString)

f.write("\n            //#########  STATELABEL END ############\n\n")
stringy = '            Console.WriteLine("' + r'\n' + 'Current state is: " + stateLabels[currentState] + ' + r'"\n"' + ');\n'
f.write(stringy)
f.write("        }\n\n")
f.write("        public bool On(string eventoLabel)\n")
f.write("        {\n")
f.write("            evento = eventLabels[eventoLabel];\n")
f.write("            if (transiciones.ContainsKey((currentState, evento)))\n")
f.write("            {\n")
f.write("                currentState = transiciones[(currentState, evento)];\n")

if bool(noncontrollableEvents):
    stringy = "                if ("
    for count,event in enumerate(noncontrollableEvents):
        stringy = stringy + "evento != " + event
        if count != len(noncontrollableEvents) - 1:            
            stringy = stringy + " && "
    stringy = stringy + ")\n"
    f.write(stringy)
    f.write('                {\n')
    f.write('                    Console.WriteLine(eventoLabel + " event approved");\n')
    f.write('                }\n')
    f.write('                else\n')
    f.write('                {\n')
    f.write('                    Console.WriteLine(eventoLabel + " event is uncontrollable and must be enabled");\n')
    f.write('                }\n')
else:
    f.write('                    Console.WriteLine(eventoLabel + " event approved");\n')



stringy = r'                Console.WriteLine("Current state is: " + stateLabels[currentState] + "\n");'
f.write(stringy + '\n')
f.write('                return true;\n')
f.write('            } else\n')
f.write('            {\n')
f.write('                Console.WriteLine(eventoLabel + " event blocked");\n')
f.write('                Thread.Sleep(800);\n')
f.write('                return false;\n')
f.write('            }\n')
f.write('        }\n')
f.write('    }\n')
f.write('}')
f.close()
    
print("\n\nFile named " + csFileName + " created succesfully.\n\n")
##############  PROGRAM WRITING ENDS  ########################
