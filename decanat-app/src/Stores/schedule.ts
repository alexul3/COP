import { defineStore } from 'pinia'
import { ref } from 'vue'
import api, { type ScheduleDto, type CreateScheduleDto } from '@/services/api'

export const useScheduleStore = defineStore('schedule', () => {
  const schedules = ref<ScheduleDto[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  async function loadSchedules() {
    isLoading.value = true
    error.value = null
    
    try {
      schedules.value = await api.getAllSchedules()
    } catch (err: any) {
      error.value = 'Ошибка загрузки расписания'
      console.error(err)
    } finally {
      isLoading.value = false
    }
  }

  async function createSchedule(schedule: CreateScheduleDto) {
    try {
      const newSchedule = await api.createSchedule(schedule)
      schedules.value.push(newSchedule)
      return newSchedule
    } catch (err: any) {
      error.value = err.response?.data || 'Ошибка создания расписания'
      throw err
    }
  }

  async function deleteSchedule(id: number) {
    try {
      await api.deleteSchedule(id)
      schedules.value = schedules.value.filter(s => s.id !== id)
    } catch (err: any) {
      error.value = 'Ошибка удаления расписания'
      throw err
    }
  }

  return {
    schedules,
    isLoading,
    error,
    loadSchedules,
    createSchedule,
    deleteSchedule
  }
})