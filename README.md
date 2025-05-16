Poultry hold a significant positon in agriculture and is one of the consumed products. There is a huge demand for these products and relying on natural methods for hatching eggs proves to be inefficient, causing the development of technologies to increase the poultry production. This project focuses on the development of an egg incubator management system for Zaone poultry farm. the design will ensure optimal environment conditions for successful egg hatching. Useful parameters such as temperature, humidity and rotation of the eggs are maintained at their optimal levels to enhance hatchability rates. The system consists of multiple technologies such as sensors and micro-controllers, whereby the micro-controllers are able to control different environmental conditions of the incubators such as temperature and humidity of the incubator based on the user input feedback or feedback from the sensors such as DHT 22. The system can be remotely monitored via an android or desktop application. Chicken eggs will be used as a case study for testing and evaluating the system and there is high assurance that the system can accurately hatch eggs and prevent other problems that might harm the eggs.



# 11. Appendices

## 11.1 Appendix A - User Manual

### Create Account

**Figure 24** shows a create account page.

The farmer will have to create an account first by filling in the registration form shown in figure 21 above. The user will have to use their real email as this will be used for notification purposes. When the user clicks submit, their details are kept in a database to be used during login.

---

### Login

**Figure 25** shows a login page.

As shown in figure 22 above, the farmer will have to login by inputting the credentials which were used during registration.

---

**Figure 26** shows the navigation menu of the system.

The menu will be able to guide the user to navigate throughout the system.

---

This is where the farmer will have to add or select an already saved incubator as shown in figure 27 above. This will comprise an incubator name and capacity of the incubator.

---

The farmers will have to add egg batches through this page as shown in figure 28 above. This will be done by adding the number of eggs but it will not have to go beyond the incubator capacity else an error message will display on the screen as shown in figure 29 below.

**Figure 29** shows an error message if the entered number of eggs exceeds the incubator capacity number.

---

**Figure 30** shows the page where all added incubators will be seen.

As shown in figure 30 above, this is the first page that will display after the farmers have logged in. The page will show the incubator name and other details related to that incubator.

---

This is the page where the farmers will be able to set the temperature and humidity range. These values will be kept in Firebase database and they will be used to control the status of the incubators.

---

**Figure 32** shows the designed incubator.

Figure 32 shows the egg incubator management system for Zaone farm.

---

**Figure 33** shows a blower of the incubator.

When the farmers set the desired humidity as shown in figure 31 above, for example 62%, and the measured humidity is higher than the saved humidity, the blower shown in figure 34 above will be automatically turned on. If the measured humidity is lower than the saved humidity, the blower will be automatically turned off.

---

**Figure 34** shows the inside of the incubator.

As shown in figure 35 above, this is the inside part of the incubator where the eggs will be placed. The inside part contains a heater (light), a blower, as well as the rotating rods.

---

**Figure 35** shows temperature and humidity being saved.

The temperature has been set on 37.3 degree Celsius as shown in figure 35 above and if the temperature measured is above this saved temperature, the incubator heater will be turned off automatically. If the measured temperature is below the saved temperature, the heater will be automatically turned on.

---

**Figure 36** shows the incubator while the heater is off.

**Figure 37** above shows the status of the incubator when the temperature is high, therefore the heater (light) will be turned off automatically.

---

**Figure 37** shows the incubator heater turned on.

As shown in figure 38 above, when the measured temperature by the DHT22 sensor is low, the heater (light) will be automatically turned on.

---

**Figure 38** shows the process of monitoring temperature.

**Figure 39** shows the process of measuring temperature.

The figures above, 38 and 39, show temperature being measured using a DHT22 sensor and the temperature values will be displayed on the mobile application in real-time.

---

**Figure 40** shows the measuring process of humidity.

As shown in figure 40 above, humidity will be measured by the DHT22 sensor, and this was done using an ice block to increase the humidity.

---

**Figure 41** shows egg rotation.

The figure 41 above represents the rotation of the eggs which was set 2 minutes in figure 35 above. This means the rotation is being done every 2 minutes.

---

**Figure 42** shows an email notification.

As shown in figure 42 above, the farmers will be able to receive email notifications when the incubation period is over.

---

**Figure 43** shows a page to input the number of hatched eggs.

As shown in figure 43 above, after the farmers have received an email notification notifying them that an incubation period of a particular incubator is over, the farmers will be requested to input the number of hatched eggs so that the system is able to create reports.

---

**Figure 44** shows a line chart.

Figure 44 shows a line chart of a particular incubator and it displays the average temperature and humidity for the course of 21 days for each day.

---

**Figure 45** shows a pie chart.

As shown in figure 45 above, the pie chart demonstrates the overall hatchability rate of a particular incubator; this includes the number of hatched eggs and unhatched eggs.
