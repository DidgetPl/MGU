import os

import matplotlib.pyplot as plt
import numpy as np
import seaborn as sns
from sklearn.metrics import classification_report, confusion_matrix
from tensorflow.keras import layers, models
from tensorflow.keras.applications import MobileNetV2
from tensorflow.keras.preprocessing.image import ImageDataGenerator

folder = os.path.dirname(os.path.abspath(__file__))

train_gen = ImageDataGenerator(rescale=1./255,
                               rotation_range=15,
                               zoom_range=0.1,
                               horizontal_flip=True)

val_gen = ImageDataGenerator(rescale=1./255)

train_data = train_gen.flow_from_directory(
    folder + '\\labeled_images\\img',
    target_size=(224, 224),
    class_mode='categorical',
    shuffle=True
)

val_data = val_gen.flow_from_directory(
    folder + '\\labeled_images\\img',
    target_size=(224, 224),
    class_mode='categorical',
    shuffle=False
)

base_model = MobileNetV2(weights='imagenet', include_top=False, input_shape=(224, 224, 3))
base_model.trainable = False

model = models.Sequential([
    base_model,
    layers.GlobalAveragePooling2D(),
    layers.Dense(128, activation='relu'),
    layers.Dropout(0.25),
    layers.Dense(128, activation='relu'),
    layers.Dropout(0.25),
    layers.Dense(128, activation='relu'),
    layers.Dropout(0.25),
    layers.Dense(128, activation='relu'),
    layers.Dropout(0.25),
    layers.Dense(train_data.num_classes, activation='softmax')
])

model.compile(optimizer='adam', loss='categorical_crossentropy', metrics=['accuracy'])

model.fit(train_data, epochs=100)

y_pred_probs = model.predict(val_data)
y_pred = np.argmax(y_pred_probs, axis=1)
y_true = val_data.classes
class_labels = list(val_data.class_indices.keys())

cm = confusion_matrix(y_true, y_pred)
print("Raport klasyfikacji:")
print(classification_report(y_true, y_pred, target_names=class_labels))

plt.figure(figsize=(8, 6))
sns.heatmap(cm, annot=True, fmt='d', xticklabels=class_labels, yticklabels=class_labels, cmap='Blues')
plt.xlabel('Predicted Label')
plt.ylabel('True Label')
plt.title('Macierz Pomyłek')
plt.tight_layout()
plt.show()

model.save(folder + '\\plane_classifier.h5')