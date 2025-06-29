<style lang="scss" scoped>
@use '@/assets/variables.scss' as *;
.form-background {
  position: absolute;
  width: 100vw;
  height: 100vh;
  background-color: rgba($white, 0.05);
  backdrop-filter: blur(5px);
  left: 0;
  top: 0;
  z-index: 9;
}
.form {
  padding: 20px;
  position: absolute;
  background-color: $white;
  z-index: 99;
  border: 2px solid $black;
  width: 80%;
  left: calc(10% + 32px);
  top: 130px;
  box-shadow: -7px 11px $black;
  animation: FormAppear 0.3s;

  .inputs-area {
    display: flex;
    align-items: end;
    justify-content: space-between;
    flex-wrap: wrap;
    width: 100%;
  }

  .form-input-box {
    width: calc((100% / 3) - 10px);
    margin-bottom: 10px;
  }

  .form-buttons {
    display: flex;
    justify-content: end;
    gap: 8px;
  }

  @keyframes FormAppear {
    0% {
      box-shadow: 0px 0px $black;
    }

    100% {
      box-shadow: -7px 11px $black;
    }
  }
}
</style>

<template>
  <div v-if="isFormOpen" class="form-background">
    <form @submit.prevent="onSubmit" class="form">
      <div class="flex inputs-area">
        <template v-for="field in fields" :key="field.fieldName">
          <div class="form-input-box" v-if="isSimpleField(field)">
            <component
              :is="getComponent(field)"
              :id="field.fieldName"
              :label="field.fieldName"
              v-model="form[field.fieldName]"
              :maxlength="field.fieldMaxSize ? Number(field.fieldMaxSize) : undefined"
              :type="getInputType(field)"
              variant="inverted"
              :accept="field.dataType === FieldDataTypeEnum.Image || field.dataType === FieldDataTypeEnum.ImageArray ? 'image/*' : undefined"
              :multiple="field.dataType === FieldDataTypeEnum.ImageArray || field.dataType === FieldDataTypeEnum.OptionsArray"
              :required=field.isRequired
            />
          </div>
          <div class="form-input-box" v-if="field.dataType === FieldDataTypeEnum.OptionsArray">
            <FormMultipleSelect
              :id="field.fieldName"
              :label="field.fieldName"
              v-model="form[field.fieldName]"
              @update:modelValue="form[field.fieldName] = $event"
              :options="field.optionsArray"
              variant="inverted"
              :required=field.isRequired
            />
          </div>

          <div class="form-input-box" v-if="field.dataType === FieldDataTypeEnum.Options">
            <FormSelect
              :id="field.fieldName"
              :label="field.fieldName"
              v-model="form[field.fieldName]"
              @update:modelValue="form[field.fieldName] = $event"
              :options="field.optionsArray"
              variant="inverted"
              :required=field.isRequired
            />
          </div>

          <div class="form-input-box" v-if="field.dataType === FieldDataTypeEnum.Image">
            <FormInputImage
              :id="field.fieldName"
              :label="field.fieldName"
              v-model="form[field.fieldName]"
              @update:modelValue="form[field.fieldName] = $event"
              :options="field.optionsArray"
              variant="inverted"
              :required=field.isRequired
              :previousImage="imageField[field.fieldName]"
            />
          </div>
        </template>
      </div>
      <div class="form-buttons">
        <FormSubmit :value="data?.id ? 'Save' : 'Create'" />
        <FormButton inverted v-if="data?.id" @click="onCancel">Cancelar</FormButton>
      </div>

      <Loader v-if="isLoading" />
      <ErrorMessage variant="white" :onClose="onCloseErrorMessage" v-if="errorMessage" :message="errorMessage" />
    </form>
  </div>
</template>

<script lang="ts" setup>
import { reactive, ref, watch } from 'vue'
import FormInput from '@/components/forms/formInput.vue'
import FormSubmit from '@/components/forms/formSubmit.vue'
import FormInputImage from '@/components/forms/formInputImage.vue'
import ErrorMessage from '@/components/errorMessage.vue'
import Loader from '@/components/loader.vue'
import FormButton from '@/components/forms/formButton.vue'
import { fileToBase64 } from '@/utis/converter'
import { ApiResponseData, getErrorMessage } from '@/api/defaultApi'
import { FormFieldModel, FieldDataTypeEnum } from '@/models/formFieldModel'
import FormMultipleSelect from '../forms/formMultipleSelect.vue'
import FormSelect from '../forms/formSelect.vue'

interface Props<T = ApiResponseData> {
  data?: T;
  onComplete: () => void;
  onCancel: () => void;
  addItem: (data: T) => void;
  updateItem: (data: T) => void;
  fields?: FormFieldModel[];
  maxImageSize: number;
  isFormOpen: boolean;
}
const props = defineProps<Props>()

const form = reactive<Record<string, any>>({})
const imageField = reactive<Record<string, any>>({})
const isLoading = ref(false)
const errorMessage = ref<string | null>(null)

const onCloseErrorMessage = () => {
  errorMessage.value = null
}

const isSimpleField = (field: FormFieldModel) => {
  const dataType = field.dataType;
  
  return [FieldDataTypeEnum.Number, FieldDataTypeEnum.String, FieldDataTypeEnum.Date]
    .includes(dataType)
}

const getComponent = (field: FormFieldModel) => {
  switch (field.dataType) {
    case FieldDataTypeEnum.Image:
    case FieldDataTypeEnum.ImageArray:
      return FormInputImage
    case FieldDataTypeEnum.OptionsArray:
      return FormMultipleSelect
    default:
      return FormInput
  }
}

const getInputType = (field: FormFieldModel) => {
  switch (field.dataType) {
    case FieldDataTypeEnum.Number: return 'number'
    case FieldDataTypeEnum.Date: return 'date'
    default: return 'text'
  }
}

const onSubmit = async () => {
  errorMessage.value = null;
  if(!props.fields) {
    return;
  }
  for (const f of props.fields) {
    if (f.dataType === FieldDataTypeEnum.Image || f.dataType === FieldDataTypeEnum.ImageArray) {
      const fileOrFiles = form[f.fieldName]
      const files = Array.isArray(fileOrFiles) ? fileOrFiles : [fileOrFiles]
      for (const file of files) {
        if (file && file.size / (1024 * 1024) > props.maxImageSize / 100) {
          errorMessage.value = `The max size for pictures is ${props.maxImageSize}KB`
          return
        }
      }
    }
  }

  isLoading.value = true
  try {
    const sendData: ApiResponseData = {}

    for (const f of props.fields) {
      if (f.dataType === FieldDataTypeEnum.Image) {
        console.log(imageField[f.fieldName] )
        sendData[f.fieldName] = form[f.fieldName] ? await fileToBase64(form[f.fieldName]) : (imageField[f.fieldName] || null)
      } else if (f.dataType === FieldDataTypeEnum.ImageArray) {
        if (Array.isArray(form[f.fieldName])) {
          const arrBase64 = []
          for (const file of form[f.fieldName]) {
            if (file) arrBase64.push(await fileToBase64(file))
          }
          sendData[f.fieldName] = arrBase64
        } else sendData[f.fieldName] = []
      } else if(f.dataType === FieldDataTypeEnum.OptionsArray) {
        sendData[f.fieldName + 'Ids'] = form[f.fieldName]
      } else {
        sendData[f.fieldName] = form[f.fieldName]
      }
    }

    if(props.data?.id) {
      await props.updateItem({...sendData, id: props.data.id});
    } else {
      await props.addItem(sendData);
    }
    props.onComplete();
  } catch (e) {
    errorMessage.value = getErrorMessage(e)|| 'Error on sending the data.';
  } finally {
    isLoading.value = false;
  }
}

watch(() => props.data, (newData) => {
  if(!newData?.id) {
    for (const key in form) {
      form[key] = null;
    }
    return;
  }

  for (const field of props.fields ?? []) {
    const key = field.fieldName;
    
    if (field.dataType === FieldDataTypeEnum.OptionsArray) {
      const arr = (newData as any)[key];
      if (Array.isArray(arr)) {
        form[key] = arr.map((item: any) => item.id.toString());
      } else {
        form[key] = [];
      }
    } else if (field.dataType === FieldDataTypeEnum.Options) {
      const obj = (newData as any)[key.replace('id', '')];
      form[key] = obj?.id.toString();
    } else if (field.dataType === FieldDataTypeEnum.Image) {
      form[key] = null;
      imageField[key] = newData[key];
    }else {
      form[key] = (newData as any)[key];
    }
  }
}, { immediate: true });
</script>
