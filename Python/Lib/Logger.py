class Logger:
    def __init__(self, shell):
        caption = shell.title
        self.__filename = shell.GetSaveFileName(caption + '_params', ext='log')
        self.__lines = []

        self.AddGenericEntry(f'Sample = {shell.sample_name}, voltage = {shell.voltage}')

    def AddGenericEntry(self, text):
        self.__lines.append(text + '\n')

    # def AddParametersEntry(self, swept_caption, swept_value, swept_units, **params):
    #     strAdd = f'{swept_caption} = {swept_value} {swept_units}'
    #     for p, v in params.items():
    #         strAdd += f'; {p} = {v}'
    #     self.AddGenericEntry(strAdd)

    def Save(self):
        with open(self.__filename, 'w') as f:
            for line in self.__lines:
                f.write(line)
        print('Log was saved to:', self.__filename)
