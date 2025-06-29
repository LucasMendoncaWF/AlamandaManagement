<style lang="scss" scoped>
.team-form {
  padding: 20px;
  align-items: end;
  justify-content: space-between;
  flex-wrap: wrap;

  .team-input {
    width: calc(25% - 10px);
    margin-bottom: 10px;
  }

  .team-submit {
    margin-bottom: 10px;
    width: calc(25% - 10px);
  }
}
</style>

<template>
  <form @submit.prevent="onSubmit" class="flex team-form">
    <template v-for="field in fields" :key="field.fieldName">
      <div class="team-input" v-if="!isArrayField(field)">
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
      <div class="team-input" v-if="isMultipleSelectField(field)">
        <component
          :is="getComponent(field)"
          :id="field.fieldName"
          :label="field.fieldName"
          v-model="form[field.fieldName]"
          @update:modelValue="form[field.fieldName] = $event"
          :options="field.optionsArray"
          variant="inverted"
          :required=field.isRequired
        />
      </div>
    </template>

    <div class="team-submit">
      <FormSubmit :value="data?.id ? 'Salvar' : 'Criar'" />
      <FormButton @click="onCancel">Cancelar</FormButton>
    </div>

    <Loader v-if="isLoading" />
    <ErrorMessage variant="white" :onClose="onCloseErrorMessage" v-if="errorMessage" :message="errorMessage" />
  </form>
</template>

<script lang="ts" setup>
import { reactive, ref } from 'vue'
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

interface Props<T = ApiResponseData> {
  data?: T;
  onComplete: () => void;
  onCancel: () => void;
  addItem: (data: T) => void;
  fields?: FormFieldModel[];
}
const props = defineProps<Props>()

const form = reactive<Record<string, any>>({})
const isLoading = ref(false)
const errorMessage = ref<string | null>(null)

const onCloseErrorMessage = () => {
  errorMessage.value = null
}

const isMultipleSelectField = (field: FormFieldModel) => {
  return field.dataType === FieldDataTypeEnum.OptionsArray
}

const isArrayField = (field: FormFieldModel) => {
  return field.dataType === FieldDataTypeEnum.ImageArray || field.dataType === FieldDataTypeEnum.OptionsArray
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
  errorMessage.value = null
  if(!props.fields) {
    return
  }
  for (const f of props.fields) {
    if (f.dataType === FieldDataTypeEnum.Image || f.dataType === FieldDataTypeEnum.ImageArray) {
      const fileOrFiles = form[f.fieldName]
      const files = Array.isArray(fileOrFiles) ? fileOrFiles : [fileOrFiles]
      for (const file of files) {
        if (file && file.size / (1024 * 1024) > 0.2) {
          errorMessage.value = 'O máximo permitido para a foto é 200KB'
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
        sendData[f.fieldName] = form[f.fieldName] ? await fileToBase64(form[f.fieldName]) : null
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

    await props.addItem(sendData)
    props.onComplete();
  } catch (e) {
    errorMessage.value = getErrorMessage(e)|| 'Erro no envio';
  } finally {
    isLoading.value = false;
  }
}
</script>
