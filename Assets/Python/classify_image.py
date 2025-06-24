import os
import sys

import numpy as np
from tensorflow.keras.models import load_model
from tensorflow.keras.preprocessing.image import img_to_array, load_img

folder = os.path.dirname(os.path.abspath(__file__))

model = load_model(folder + '\\plane_classifier.h5')
labels = ["A50", "MiG-29", "Su-25", "Su-27", "Su-34", "Su-57", "Tu-160", "Tu-95"]

img_path = sys.argv[1]
img = load_img(img_path, target_size=(224, 224))
x = img_to_array(img) / 255.0
x = np.expand_dims(x, axis=0)
preds = model.predict(x)
label = labels[np.argmax(preds)]
print(label)
